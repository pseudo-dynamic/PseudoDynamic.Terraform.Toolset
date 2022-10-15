using PseudoDynamic.Terraform.Plugin.Internals.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal record class TerraformCommand
    {
        private const string TerraformCommandName = "terraform";

        private readonly static Dictionary<string, string> EmptyEnvironmentVariables = new Dictionary<string, string>();

        public string? WorkingDirectory { get; init; }

        public IReadOnlyDictionary<string, string> EnvironmentVariables { get; init; } = EmptyEnvironmentVariables;

        public TerraformReattachProviders TerraformReattachProviders { get; } = new TerraformReattachProviders();

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

            if (TerraformReattachProviders.Count > 0)
            {
                startInfo.EnvironmentVariables.Add(TerraformReattachProviders.VariableName, TerraformReattachProviders.ToJson());
            }

            return startInfo;
        }

        private string RunCommandThenReadOutput(string? args) => SimpleProcess.StartThenWaitForExitThenReadOutput(
            CreateStartInfo(args),
            shouldThrowOnNonZeroCode: true);

        public string Init() => RunCommandThenReadOutput("init");

        public string Validate() => RunCommandThenReadOutput("validate");

        public string Plan() => RunCommandThenReadOutput("plan -input=false");

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
