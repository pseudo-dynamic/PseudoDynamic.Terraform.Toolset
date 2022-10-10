namespace PseudoDynamic.Terraform.Plugin.Internals.Diagnostics
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

        public async Task<int> WaitForExitAsync()
        {
            EnsureProcessStarted();

            if (!ShouldStreamOutput)
            {
                string endAsync = await Process.StandardOutput.ReadToEndAsync();
                ReceiveOutput(endAsync);
            }

            if (!ShouldStreamError)
            {
                string endAsync = await Process.StandardError.ReadToEndAsync();
                ReceiveError(endAsync);
            }

#if NET5_0_OR_GREATER
            await Process.WaitForExitAsync();
#endif
            await Task.Run(new Action(Process.WaitForExit));
            ThrowOnNonZeroExitCode();
            return Process.ExitCode;
        }
    }
}
