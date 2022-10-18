using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <summary>
    /// Represents the aquivalent of <see cref="Process"/>
    /// </summary>
    public class SimpleProcess : IDisposable, ISimpleProcess
    {
        #region factory methods

        /// <summary>
        /// Starts the process and then waits for the exit.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="outputReceived"></param>
        /// <param name="shouldStreamOutput"></param>
        /// <param name="errorReceived"></param>
        /// <param name="shouldStreamError"></param>
        /// <param name="shouldThrowOnNonZeroCode"></param>
        /// <returns>The exit code.</returns>
        public static int StartThenWaitForExit(
          SimpleProcessStartInfo startInfo,
          Action<string?>? outputReceived = null,
          bool shouldStreamOutput = false,
          Action<string?>? errorReceived = null,
          bool shouldStreamError = false,
          bool shouldThrowOnNonZeroCode = false)
        {
            using SimpleProcess simpleProcess = new SimpleProcess(
                startInfo,
                outputReceived,
                shouldStreamOutput,
                errorReceived,
                shouldStreamError,
                shouldThrowOnNonZeroCode);

            simpleProcess.Start();
            return simpleProcess.WaitForExit();
        }

        /// <summary>
        /// Starts the process and then waits for it exit asynchronously.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="outputReceived"></param>
        /// <param name="shouldStreamOutput"></param>
        /// <param name="errorReceived"></param>
        /// <param name="shouldStreamError"></param>
        /// <param name="shouldThrowOnNonZeroCode"></param>
        /// <returns>The exit code.</returns>
        public static async Task<int> StartThenWaitForExitAsync(
          SimpleProcessStartInfo startInfo,
          Action<string?>? outputReceived = null,
          bool shouldStreamOutput = false,
          Action<string?>? errorReceived = null,
          bool shouldStreamError = false,
          bool shouldThrowOnNonZeroCode = false)
        {
            using var process = new SimpleAsyncProcess(
                startInfo,
                outputReceived,
                shouldStreamOutput,
                errorReceived,
                shouldStreamError,
                shouldThrowOnNonZeroCode);

            process.Start();
            return await process.WaitForExitAsync();
        }

        /// <summary>
        /// Starts the process, waits for the exit and provdes the output the process has made.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="errorReceived"></param>
        /// <param name="shouldStreamError"></param>
        /// <param name="shouldThrowOnNonZeroCode"></param>
        /// <returns>The read output.</returns>
        public static string StartThenWaitForExitThenReadOutput(
          SimpleProcessStartInfo startInfo,
          Action<string?>? errorReceived = null,
          bool shouldStreamError = false,
          bool shouldThrowOnNonZeroCode = false)
        {
            StringBuilder outputBuilder = new StringBuilder();

            using SimpleProcess simpleProcess = new SimpleProcess(
                startInfo,
                output => outputBuilder.Append(output),
                errorReceived: errorReceived,
                shouldStreamError: shouldStreamError,
                shouldThrowOnNonZeroCode: shouldThrowOnNonZeroCode);

            simpleProcess.Start();
            simpleProcess.WaitForExit();
            return outputBuilder.ToString();
        }

        /// <summary>
        /// Starts the process, waits for the exit and provdes the output the process has made.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="errorReceived"></param>
        /// <param name="shouldStreamError"></param>
        /// <param name="shouldThrowOnNonZeroCode"></param>
        /// <returns>The read output.</returns>
        public static async Task<string> StartThenWaitForExitThenReadOutputAsync(
          SimpleProcessStartInfo startInfo,
          Action<string?>? errorReceived = null,
          bool shouldStreamError = false,
          bool shouldThrowOnNonZeroCode = false)
        {
            StringBuilder outputBuilder = new StringBuilder();

            using var process = new SimpleAsyncProcess(
                startInfo,
                output => outputBuilder.Append(output),
                errorReceived: errorReceived,
                shouldStreamError: shouldStreamError,
                shouldThrowOnNonZeroCode: shouldThrowOnNonZeroCode);

            process.Start();
            _ = await process.WaitForExitAsync();
            return outputBuilder.ToString();
        }

        #endregion

        private readonly StringBuilder _errorBuilder;
        private readonly ProcessStartInfo _processStartInfo;
        private bool _isDisposed;

        /// <summary>
        /// True if process has been started.
        /// </summary>
        [MemberNotNullWhen(true, "Process")]
        public bool IsProcessStarted { get; private set; }
        /// <summary>
        /// <see langword="true"/> enables streaming the output the process may produce.
        /// </summary>
        public bool ShouldStreamOutput { get; }
        /// <summary>
        /// <see langword="true"/> enables streaming the error the process may produce.
        /// </summary>
        public bool ShouldStreamError { get; }
        /// <summary>
        /// Throw an instance of <see cref="NonZeroExitCodeException"/> in case of a on non-zero exit code.
        /// </summary>
        public bool ShouldThrowOnNonZeroCode { get; }

        /// <summary>
        /// The exit code of underlying process.
        /// </summary>
        public int ExitCode
        {
            get
            {
                EnsureProcessCreated();
                return Process.ExitCode;
            }
        }

        /// <summary>
        /// The underlying process.
        /// </summary>
        protected Process? Process { get; private set; }
        private Action<string?>? _outputReceived { get; }
        private Action<string?>? _errorReceived { get; }

        // ISSUE: Streaming does not work in Ubuntu.
        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="processStartInfo"></param>
        /// <param name="outputReceived"></param>
        /// <param name="shouldStreamOutput"></param>
        /// <param name="errorReceived"></param>
        /// <param name="shouldStreamError"></param>
        /// <param name="shouldThrowOnNonZeroCode"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SimpleProcess(
          SimpleProcessStartInfo processStartInfo,
          Action<string?>? outputReceived = null,
          bool shouldStreamOutput = false,
          Action<string?>? errorReceived = null,
          bool shouldStreamError = false,
          bool shouldThrowOnNonZeroCode = false)
        {
            if (processStartInfo == null)
            {
                throw new ArgumentNullException(nameof(processStartInfo));
            }

            _errorBuilder = new StringBuilder();
            _processStartInfo = processStartInfo.ProcessStartInfo;
            _outputReceived = outputReceived;
            ShouldStreamOutput = shouldStreamOutput;
            _errorReceived = errorReceived;
            ShouldStreamError = shouldStreamError;
            ShouldThrowOnNonZeroCode = shouldThrowOnNonZeroCode;
        }

        [MemberNotNull("Process")]
        private void AttachProcessHandlers()
        {
            EnsureProcessCreated();
            Process.Exited += OnProcessExited;

            if (ShouldStreamOutput)
            {
                Process.OutputDataReceived += OnOutputDataReceived;
            }

            if (!ShouldStreamError)
            {
                return;
            }

            Process.ErrorDataReceived += OnErrorDataReceived;
        }

        [MemberNotNull("Process")]
        private void DettachProcessHandlers()
        {
            EnsureProcessCreated();
            Process.Exited -= OnProcessExited;

            if (ShouldStreamOutput)
            {
                Process.OutputDataReceived -= OnOutputDataReceived;
            }

            if (!ShouldStreamError)
            {
                return;
            }

            Process.ErrorDataReceived -= OnOutputDataReceived;
        }

        /// <summary>
        /// Called when the process exited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProcessExited(object? sender, EventArgs e) =>
            DettachProcessHandlers();

        /// <summary>
        /// Called when output received.
        /// </summary>
        /// <param name="data"></param>
        protected void ReceiveOutput(string? data) =>
            _outputReceived?.Invoke(data);

        private void OnOutputDataReceived(object? sender, DataReceivedEventArgs e) =>
            ReceiveOutput(e.Data);

        /// <summary>
        /// Called when exception received.
        /// </summary>
        /// <param name="data"></param>
        protected void ReceiveError(string? data)
        {
            _errorBuilder?.Append(data);
            _errorReceived?.Invoke(data);
        }

        private void OnErrorDataReceived(object? sender, DataReceivedEventArgs e) =>
            ReceiveError(e.Data);

        /// <summary>Prepares the process.</summary>
        /// <exception cref="InvalidOperationException">Process alreay created.</exception>
        [MemberNotNull("Process")]
        protected virtual void CreateProcess()
        {
            if (Process is not null)
            {
                throw new InvalidOperationException("Process alreay created.");
            }

            Process = new Process()
            {
                StartInfo = _processStartInfo
            };

            Process.EnableRaisingEvents = true;
            AttachProcessHandlers();
        }

        /// <summary>
        /// Factory method for <see cref="NonZeroExitCodeException"/>.
        /// </summary>
        protected NonZeroExitCodeException CreateNonZeroExitCodeException()
        {
            EnsureProcessCreated();
            bool isErrorEmpty = _errorBuilder.Length == 0;
            _errorBuilder.Insert(0, ProcessStartInfoUtils.GetExecutionInfoText(_processStartInfo) + (isErrorEmpty ? "" : Environment.NewLine));
            return new NonZeroExitCodeException(Process.ExitCode, _errorBuilder.ToString());
        }

        /// <summary>When an exception occurred during process start.</summary>
        /// <param name="exception"></param>
        protected virtual void OnProcessNotStarted(Exception exception)
        {
        }

        private static Exception CreateProcessNotStartedException() => new ProcessNotSpawnedException();

        /// <summary>Starts the process.</summary>
        /// <exception cref="InvalidOperationException">Process already created.</exception>
        public void Start()
        {
            CreateProcess();
            bool isProcessStarted;

            try
            {
                isProcessStarted = Process.Start();
            }
            catch (Exception ex)
            {
                OnProcessNotStarted(ex);
                throw;
            }

            if (!isProcessStarted)
            {
                OnProcessNotStarted(CreateProcessNotStartedException());
            }

            if (ShouldStreamOutput)
            {
                Process.BeginOutputReadLine();
            }

            if (ShouldStreamError)
            {
                Process.BeginErrorReadLine();
            }

            IsProcessStarted = true;
        }

        /// <summary>Ensures process has been created.</summary>
        /// <exception cref="InvalidOperationException">Process not created yet.</exception>
        [MemberNotNull("Process")]
        protected void EnsureProcessCreated()
        {
            if (Process == null)
            {
                throw new InvalidOperationException("Process not created yet.");
            }
        }

        /// <summary>Ensures process has been started.</summary>
        /// <exception cref="InvalidOperationException">Process not started yet.</exception>
        [MemberNotNull("Process")]
        protected void EnsureProcessStarted()
        {
            if (!IsProcessStarted)
            {
                throw new InvalidOperationException("Process not started yet.");
            }
        }

        /// <summary>Throws if exit code is non-zero.</summary>
        /// <exception cref="NonZeroExitCodeException"></exception>
        protected void ThrowOnNonZeroExitCode()
        {
            if (!ShouldThrowOnNonZeroCode)
            {
                return;
            }

            EnsureProcessCreated();

            if (Process.ExitCode != 0)
            {
                throw CreateNonZeroExitCodeException();
            }
        }

        /// <summary>Waits for exit.</summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NonZeroExitCodeException"></exception>
        public int WaitForExit()
        {
            EnsureProcessStarted();

            if (!ShouldStreamOutput)
            {
                ReceiveOutput(Process.StandardOutput.ReadToEnd());
            }

            if (!ShouldStreamError)
            {
                ReceiveError(Process.StandardError.ReadToEnd());
            }

            Process.WaitForExit();
            ThrowOnNonZeroExitCode();
            return Process.ExitCode;
        }

        /// <inheritdoc/>
        public void Kill()
        {
            EnsureProcessStarted();
            Process.Kill();
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                DettachProcessHandlers();
                Process.Dispose();
            }

            _isDisposed = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
