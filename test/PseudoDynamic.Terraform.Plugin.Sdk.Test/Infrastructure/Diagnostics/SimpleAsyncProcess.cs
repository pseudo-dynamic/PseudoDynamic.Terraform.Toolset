namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    internal class SimpleAsyncProcess : SimpleProcess, ISimpleAsyncProcess
    {
        public SimpleAsyncProcess(
          SimpleProcessStartInfo processStartInfo,
          Action<string?>? outputReceived = null,
          bool shouldStreamOutput = false,
          Action<string?>? errorReceived = null,
          bool shouldStreamError = false,
          bool shouldThrowOnNonZeroCode = false)
          : base(
                processStartInfo,
                outputReceived,
                shouldStreamOutput,
                errorReceived,
                shouldStreamError,
                shouldThrowOnNonZeroCode)
        {
        }

        public async Task<int> WaitForExitAsync(CancellationToken cancellationToken = default)
        {
            EnsureProcessStarted();

            //if (!ShouldStreamOutput)
            //{
            //    string endAsync = await Process.StandardOutput.ReadToEndAsync();
            //    ReceiveOutput(endAsync);
            //}

            //if (!ShouldStreamError)
            //{
            //    string endAsync = await Process.StandardError.ReadToEndAsync();
            //    ReceiveError(endAsync);
            //}

            await Process.WaitForExitAsync(cancellationToken);
            await Task.Run(new Action(Process.WaitForExit));
            ThrowOnNonZeroExitCode();
            return Process.ExitCode;
        }
    }
}
