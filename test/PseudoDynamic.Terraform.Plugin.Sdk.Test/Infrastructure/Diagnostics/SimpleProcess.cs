using System.Diagnostics;
using System.Text;
using Toolbelt.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <summary>
    /// Represents the aquivalent of <see cref="Process"/>
    /// </summary>
    public static class SimpleProcess
    {
        /// <summary>
        /// Starts the process, waits for the exit and provdes the output the process has made.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The read output.</returns>
        public static async Task<string> StartThenWaitForExitThenReadOutputAsync(
          SimpleProcessStartInfo startInfo,
          CancellationToken cancellationToken = default)
        {
            StringBuilder outputBuilder = new StringBuilder();
            StringBuilder errorBuilder = new StringBuilder();

            var process = XProcess.Start(startInfo.CreateProcessStartInfo());

            var readOutput = Task.Run(async () => {
                await foreach (var line in process.GetOutputAsyncStream(cancellationToken)) {
                    outputBuilder.AppendLine(line);
                }
            });

            var readError = Task.Run(async () => {
                await foreach (var line in process.GetOutputAsyncStream(cancellationToken)) {
                    errorBuilder.AppendLine(line);
                }
            });

            await Task.WhenAll(readOutput, readError);

            if (process.ExitCode!.Value != 0) {
                throw new NonZeroExitCodeException(process.ExitCode!.Value, errorBuilder.ToString());
            }

            return outputBuilder.ToString();
        }
    }
}
