using DotNext.Buffers;
using System.Buffers;
using System.Text;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <inheritdoc/>
    public class AsyncSimpleProcess : SimpleProcess, IAsyncSimpleProcess
    {
        /// <summary>
        /// Starts the process, waits for the exit and provdes the output the process has made.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="encoding">The to be used encoding for output or error incoming bytes.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The read output.</returns>
        public static async Task<string> StartThenWaitForExitThenReadOutputAsync(
          SimpleProcessStartInfo startInfo,
          Encoding? encoding = null,
          CancellationToken cancellationToken = default)
        {
            encoding ??= Encoding.UTF8;
            using var outputBuffer = new SparseBufferWriter<byte>();
            using var errorBuffer = new SparseBufferWriter<byte>();

            using var process = new AsyncSimpleProcess(startInfo, outputBuffer, errorBuffer, cancellationToken);
            var exitCode = await process.WaitForExitAsync();

            if (exitCode != 0) {
                var error = encoding.GetString(errorBuffer.ToReadOnlySequence());
                throw new BadExitCodeException(error) { ExitCode = exitCode };
            }

            return encoding.GetString(outputBuffer.ToReadOnlySequence());
        }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="processStartInfo"></param>
        /// <param name="outputBuffer">The buffer the process will write incoming output to.</param>
        /// <param name="errorBuffer">The buffer the process will write incoming error to.</param>
        /// <param name="cancellationToken"></param>
        internal AsyncSimpleProcess(
            SimpleProcessStartInfo processStartInfo,
            IBufferWriter<byte>? outputBuffer,
            IBufferWriter<byte>? errorBuffer,
            CancellationToken cancellationToken)
            : base(processStartInfo,
                  outputBuffer,
                  errorBuffer,
                  cancellationToken)
        {
        }

        /// <inheritdoc/>
        public async Task<int> WaitForExitAsync()
        {
            CreateProcess(out var process);
            StartProcess(process);

            await Task.WhenAll(
                    process.WaitForExitAsync(UserRequestedCancellationToken),
                    ReadOutputTask,
                    ReadErrorTask)
                .ConfigureAwait(false);

            return process.ExitCode;
        }
    }
}
