using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DotNext.Buffers;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <summary>
    /// A simple process trying to replace the bootstrap code of a native instance of <see cref="Process"/>.
    /// </summary>
    public sealed class SimpleProcess : ISimpleProcess, IAsyncSimpleProcess
    {
        private static async Task ReadStreamAsync(Stream source, IBufferWriter<byte> destination, CancellationToken cancellationToken)
        {
            using IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(1024 * 4);
            int lastWrittenBytesCount = -1;

            while (!(cancellationToken.IsCancellationRequested && lastWrittenBytesCount == 0)) {
                lastWrittenBytesCount = await source.ReadAsync(memoryOwner.Memory).ConfigureAwait(false);
                destination.Write(memoryOwner.Memory.Span[..lastWrittenBytesCount]);
            }
        }

        /// <summary>
        /// Starts the process, waits for the exit and provdes the output the process has made.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="expectedExitCode"></param>
        /// <param name="encoding">The to be used encoding for output or error incoming bytes.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The read output.</returns>
        public static string StartThenWaitForExitThenReadOutput(
          SimpleProcessStartInfo startInfo,
          int expectedExitCode = 0,
          Encoding? encoding = null,
          CancellationToken cancellationToken = default)
        {
            encoding ??= Encoding.UTF8;
            using SparseBufferWriter<byte> outputBuffer = new();
            using SparseBufferWriter<byte> errorBuffer = new();

            using SimpleProcess process = new(startInfo, outputBuffer, errorBuffer, cancellationToken);
            int exitCode = process.WaitForExit();

            if (exitCode != expectedExitCode) {
                string error = encoding.GetString(errorBuffer.ToReadOnlySequence());
                throw new BadExitCodeException(error) { ExitCode = exitCode };
            }

            return encoding.GetString(outputBuffer.ToReadOnlySequence());
        }

        /// <summary>
        /// Starts the process, waits for the exit and provdes the output the process has made.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="expectedExitCode"></param>
        /// <param name="encoding">The to be used encoding for output or error incoming bytes.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The read output.</returns>
        public static async Task<string> StartThenWaitForExitThenReadOutputAsync(
          SimpleProcessStartInfo startInfo,
          int expectedExitCode = 0,
          Encoding? encoding = null,
          CancellationToken cancellationToken = default)
        {
            encoding ??= Encoding.UTF8;
            using SparseBufferWriter<byte> outputBuffer = new();
            using SparseBufferWriter<byte> errorBuffer = new();

            using SimpleProcess process = new(startInfo, outputBuffer, errorBuffer, cancellationToken);
            int exitCode = await process.WaitForExitAsync().ConfigureAwait(false);

            if (exitCode != expectedExitCode) {
                string error = encoding.GetString(errorBuffer.ToReadOnlySequence());
                throw new BadExitCodeException(error) { ExitCode = exitCode };
            }

            return encoding.GetString(outputBuffer.ToReadOnlySequence());
        }

        /// <summary>
        /// <see langword="true"/> if internal process has been started.
        /// </summary>
        [MemberNotNullWhen(true, nameof(ReadOutputTask), nameof(ReadErrorTask))]
        public bool IsProcessStarted { get; private set; }

        /// <summary>
        /// <see langword="true"/> if internal process has been started successfully.
        /// </summary>
        public bool IsProcessStartedSuccessfully { get; private set; }

        /// <summary>
        /// A token that gets cancelled when the process exits.
        /// </summary>
        public CancellationToken ProcessExited { get; }

        internal CancellationToken UserRequestedCancellationToken { get; }
        internal Task? ReadOutputTask { get; private set; }
        internal Task? ReadErrorTask { get; private set; }

        private readonly CancellationTokenSource _processExitedTokenSource;
        private readonly SimpleProcessStartInfo _processStartInfo;
        private readonly IBufferWriter<byte>? _outputBuffer;
        private readonly IBufferWriter<byte>? _errorBuffer;
        private Process? _process;
        private bool _isDisposed;
        private readonly object _startProcessLock = new();

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="processStartInfo"></param>
        /// <param name="outputBuffer">The buffer the process will write incoming output to.</param>
        /// <param name="errorBuffer">The buffer the process will write incoming error to.</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal SimpleProcess(
            SimpleProcessStartInfo processStartInfo,
            IBufferWriter<byte>? outputBuffer,
            IBufferWriter<byte>? errorBuffer,
            CancellationToken cancellationToken)
        {
            UserRequestedCancellationToken = cancellationToken;
            _processStartInfo = processStartInfo ?? throw new ArgumentNullException(nameof(processStartInfo));
            _outputBuffer = outputBuffer;
            _errorBuffer = errorBuffer;
            _processExitedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            ProcessExited = _processExitedTokenSource.Token;
        }

        internal void CreateProcess(out Process process)
        {
            Process? currentProcess = _process;

            if (currentProcess is not null) {
                process = currentProcess;
                return;
            }

            Process newProcess = new() { StartInfo = _processStartInfo.CreateProcessStartInfo() };

            if (Interlocked.CompareExchange(ref _process, newProcess, null) == null) {
                process = newProcess;
                return;
            }

            // Someone else was faster, so we dispose new process
            newProcess.Dispose();
            process = currentProcess!;
        }

        [MemberNotNull(nameof(ReadOutputTask), nameof(ReadErrorTask))]
        internal void StartProcess(Process process)
        {
            // Try non-lock version
            if (IsProcessStarted) {
                return;
            }

            lock (_startProcessLock) {
                if (IsProcessStarted) {
                    return;
                }

                process.EnableRaisingEvents = true;
                void OnProcessExited(object? sender, EventArgs e)
                {
                    process.Exited -= OnProcessExited;
                    _processExitedTokenSource.Cancel();
                }
                process.Exited += OnProcessExited;

                IsProcessStartedSuccessfully = process.Start();
                IsProcessStarted = true;

                ReadOutputTask = _outputBuffer is not null
                    ? ReadStreamAsync(process.StandardOutput.BaseStream, _outputBuffer, ProcessExited)
                    : Task.CompletedTask;

                ReadErrorTask = _errorBuffer is not null
                    ? ReadStreamAsync(process.StandardError.BaseStream, _errorBuffer, ProcessExited)
                    : Task.CompletedTask;
            }
        }

        /// <inheritdoc/>
        public bool Start()
        {
            CreateProcess(out Process? process);
            StartProcess(process);
            return IsProcessStartedSuccessfully;
        }

        /// <inheritdoc/>
        public int WaitForExit()
        {
            CreateProcess(out Process? process);
            StartProcess(process);
            process.WaitForExit();
            // This makes the assumption, that every await uses
            // ConfigureAwait(continueOnCapturedContext: false)
            Task.WaitAll(ReadErrorTask, ReadErrorTask);
            return process.ExitCode;
        }

        /// <inheritdoc/>
        public async Task<int> WaitForExitAsync()
        {
            CreateProcess(out Process? process);
            StartProcess(process);

            await Task.WhenAll(
                    process.WaitForExitAsync(UserRequestedCancellationToken),
                    ReadOutputTask,
                    ReadErrorTask)
                .ConfigureAwait(false);

            return process.ExitCode;
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed) {
                return;
            }

            if (disposing) {
                _process?.Dispose();
                _processExitedTokenSource.Dispose();
            }

            _isDisposed = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
