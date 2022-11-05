using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <summary>
    /// A simple process trying to replace the bootstrap code of a native instance of <see cref="Process"/>.
    /// </summary>
    public class SimpleProcess : ISimpleProcess
    {
        private static async Task ReadStreamAsync(Stream source, IBufferWriter<byte> destination, CancellationToken cancellationToken)
        {
            using var memoryOwner = MemoryPool<byte>.Shared.Rent(1024 * 4);
            var lastWrittenBytesCount = -1;

            while (!(cancellationToken.IsCancellationRequested && lastWrittenBytesCount == 0)) {
                lastWrittenBytesCount = await source.ReadAsync(memoryOwner.Memory).ConfigureAwait(false);
                destination.Write(memoryOwner.Memory.Span.Slice(0, lastWrittenBytesCount));
            }
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

        private CancellationTokenSource _processExitedTokenSource;
        private readonly SimpleProcessStartInfo _processStartInfo;
        private readonly IBufferWriter<byte>? _outputBuffer;
        private readonly IBufferWriter<byte>? _errorBuffer;
        private Process? _process;
        private object _startProcessLock = new();

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
            var currentProcess = _process;

            if (currentProcess is not null) {
                process = currentProcess;
                return;
            }

            var newProcess = new Process() { StartInfo = _processStartInfo.CreateProcessStartInfo() };

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
            lock (_startProcessLock) {
                if (IsProcessStarted) {
                    return;
                }

                process.EnableRaisingEvents = true;
                EventHandler? onProcessExited = default;
                onProcessExited = (object? sender, EventArgs e) => {
                    process.Exited -= onProcessExited;
                    //_ = Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(task => { _processExitedTokenSource.Cancel(); });
                    _processExitedTokenSource.Cancel();
                };
                process.Exited += onProcessExited;

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
            CreateProcess(out var process);
            StartProcess(process);
            return IsProcessStartedSuccessfully;
        }

        /// <inheritdoc/>
        public int WaitForExit()
        {
            CreateProcess(out var process);
            StartProcess(process);
            process.WaitForExit();

            return process.ExitCode;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_process is not null) {
                _process.Dispose();
            }

            _processExitedTokenSource.Dispose();
        }
    }
}
