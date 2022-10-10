using PseudoDynamic.Terraform.Plugin.Internals.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal record class TerraformCommand
    {
        private const string TerraformCommandName = "terraform";

        private readonly static Dictionary<string, string> EmptyEnvironmentVariables = new Dictionary<string, string>();

        public string? WorkingDirectory { get; init; }

        public IReadOnlyDictionary<string, string> EnvironmentVariables { get; init; } = EmptyEnvironmentVariables;

        private TerraformProcessStartInfo CreateStartInfo(string? args)
        {
            var startInfo = new TerraformProcessStartInfo(
                TerraformCommandName,
                args: args,
                workingDirectory: WorkingDirectory);

            foreach (var environmentVariable in EnvironmentVariables)
            {
                startInfo.EnvironmentVariables.Add(environmentVariable.Key, environmentVariable.Value);
            }

            return startInfo;
        }

        private string RunCommandThenReadOutput(string? args) => SimpleProcess.StartThenWaitForExitThenReadOutput(
            CreateStartInfo(args),
            shouldThrowOnNonZeroCode: true);

        public string Init() => RunCommandThenReadOutput("init");

        public string Apply() => RunCommandThenReadOutput("apply -input=false -auto-approve");

        private class TerraformProcessStartInfo : SimpleProcessStartInfo
        {
            public TerraformProcessStartInfo(string name, string? args = null, string? workingDirectory = null)
                : base(name, args, workingDirectory)
            {
            }
        }
    }
}
