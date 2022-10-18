using PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class TerraformCommand
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

        internal class WorkingDirectoryCloning : TerraformCommand, IDisposable
        {
            public new string WorkingDirectory
            {
                get => base.WorkingDirectory!;
                private init => base.WorkingDirectory = value;
            }

            private readonly WorkingDirectoryCloningOptions _options;
            private bool _isDisposed;

            public WorkingDirectoryCloning(Action<WorkingDirectoryCloningOptions>? configureOptions)
            {
                var options = new WorkingDirectoryCloningOptions();
                configureOptions?.Invoke(options);

                if (options.DeleteOnDispose
                    && options.DeleteOnlyTempDirectory
                    && !Path.IsPathFullyQualified(options.TemporaryWorkingDirectory))
                {
                    throw new ArgumentException("Temporary working directory must be fully qualified");
                }

                _options = options;

                var workingDirectory = options.WorkingDirectory != null
                    ? (Path.IsPathRooted(options.WorkingDirectory)
                        ? options.WorkingDirectory
                        : (Path.Combine(Environment.CurrentDirectory, options.WorkingDirectory)))
                    : Environment.CurrentDirectory;

                Directory.CreateDirectory(workingDirectory);
                var workingDirectoryInfo = new DirectoryInfo(workingDirectory);

                foreach (var file in workingDirectoryInfo.EnumerateFiles("*.tf", SearchOption.AllDirectories))
                {
                    var relativeDirectory = file.DirectoryName!.Remove(0, workingDirectory.Length);
                    var mirroringDirectory = Path.Combine(options.TemporaryWorkingDirectory, relativeDirectory);
                    Directory.CreateDirectory(mirroringDirectory); // May be redudant but we don't care

                    var mirroringFile = Path.Combine(mirroringDirectory, file.Name);
                    file.CopyTo(mirroringFile);
                }

                WorkingDirectory = options.TemporaryWorkingDirectory;
            }

            public WorkingDirectoryCloning(string workingDirectory)
                : this(options => options.WorkingDirectory = workingDirectory)
            {
            }

            public WorkingDirectoryCloning()
                : this(configureOptions: null)
            {
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing && _options.DeleteOnDispose)
                {
                    var gitDirectory = _options.TemporaryWorkingDirectory;

                    if (Directory.Exists(gitDirectory)
                        && (!_options.DeleteOnlyTempDirectory || ContainsBasePath(gitDirectory, Path.GetTempPath())))
                    {
                        foreach (var filePath in Directory.EnumerateFiles(gitDirectory, "*", SearchOption.AllDirectories))
                        {
                            File.SetAttributes(filePath, FileAttributes.Normal);
                        }

                        Directory.Delete(_options.TemporaryWorkingDirectory, recursive: true);
                    }

                    static bool ContainsBasePath(string subPath, string basePath)
                    {
                        var relativePath = Path.GetRelativePath(basePath, subPath);
                        return !relativePath.StartsWith('.') && !Path.IsPathRooted(relativePath);
                    }
                }

                _isDisposed = true;
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            internal class WorkingDirectoryCloningOptions
            {
                public string? WorkingDirectory { get; set; }
                public string TemporaryWorkingDirectory { get; }

                /// <summary>
                /// If true the directory gets deleted on dispose.
                /// Default is <see langword="true"/>.
                /// </summary>
                public bool DeleteOnDispose { get; set; } = true;

                /// <summary>
                /// If true the repository directory gets only deleted when 
                /// its path begins with <see cref="Path.GetTempPath"/>.
                /// No Effect if <see cref="DeleteOnDispose"/> is <see langword="false"/>.
                /// Default is <see langword="true"/>.
                /// </summary>
                public bool DeleteOnlyTempDirectory { get; set; } = true;

                public WorkingDirectoryCloningOptions() => TemporaryWorkingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
        }
    }
}
