using PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics;
using System.Text.Json;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class TerraformCommand
    {
        private static TerraformCommandOptions ProvideOptions(Action<TerraformCommandOptions>? configureOptions)
        {
            var options = new TerraformCommandOptions();
            configureOptions?.Invoke(options);
            return options;
        }


        public const string TerraformReattachProvidersVariableName = "TF_REATTACH_PROVIDERS";
        private const string TerraformCommandName = "terraform";


        public string? WorkingDirectory { get; init; }

        public IReadOnlyDictionary<string, string> EnvironmentVariables { 
            get => environmentVariables; 
            init => environmentVariables = value ?? throw new ArgumentNullException(nameof(value)); 
        }

        public IReadOnlyDictionary<string, TerraformReattachProvider>? TerraformReattachProviders { get; init; }

        private TerraformProcessStartInfo? _processStartInfoBase;
        private IReadOnlyDictionary<string, string> environmentVariables = SimpleProcessStartInfo.EmptyEnvironmentVariables;

        internal TerraformCommand(ITerraformCommandOptionsOptions options) =>
            TerraformReattachProviders = options.TerraformReattachProviders;

        public TerraformCommand(Action<TerraformCommandOptions> configureOptions)
            : this(ProvideOptions(configureOptions))
        {
        }

        private TerraformProcessStartInfo CreateBaseStartInfo()
        {
            var processStartInfoBase = _processStartInfoBase;

            if (processStartInfoBase != null)
            {
                return processStartInfoBase;
            }

            var environmentVariables = new Dictionary<string, string>(EnvironmentVariables);

            if (TerraformReattachProviders != null && TerraformReattachProviders.Count > 0)
            {
                environmentVariables.Add(TerraformReattachProvidersVariableName, JsonSerializer.Serialize(TerraformReattachProviders));
            }

            processStartInfoBase = new TerraformProcessStartInfo()
            {
                WorkingDirectory = WorkingDirectory,
                EnvironmentVariables = environmentVariables
            };

            _processStartInfoBase = processStartInfoBase;
            return processStartInfoBase;
        }

        private TerraformProcessStartInfo CreateStartInfo(string? args) => CreateBaseStartInfo() with
        {
            Arguments = args
        };

        private string RunCommandThenReadOutput(string? args) => SimpleProcess.StartThenWaitForExitThenReadOutput(
            CreateStartInfo(args),
            shouldThrowOnNonZeroCode: true);

        public string Init() => RunCommandThenReadOutput("init");

        public string Validate() => RunCommandThenReadOutput("validate");

        public string Plan() => RunCommandThenReadOutput("plan -input=false");

        public string Apply() => RunCommandThenReadOutput("apply -input=false -auto-approve");

        public interface ITerraformCommandOptionsOptions
        {
            IReadOnlyDictionary<string, TerraformReattachProvider> TerraformReattachProviders { get; }
        }

        public class TerraformCommandOptionsBase<DerivedOptions> : ITerraformCommandOptionsOptions
            where DerivedOptions : TerraformCommandOptionsBase<DerivedOptions>
        {
            Dictionary<string, TerraformReattachProvider> TerraformReattachProviders { get; } = new Dictionary<string, TerraformReattachProvider>();

            IReadOnlyDictionary<string, TerraformReattachProvider> ITerraformCommandOptionsOptions.TerraformReattachProviders => TerraformReattachProviders;

            protected TerraformCommandOptionsBase()
            {
            }

            public DerivedOptions WithReattachingProvider(string providerName, TerraformReattachProvider provider)
            {
                TerraformReattachProviders.Add(providerName, provider);
                return (DerivedOptions)this;
            }
        }

        public class TerraformCommandOptions : TerraformCommandOptionsBase<TerraformCommandOptions>
        {
        }

        private record class TerraformProcessStartInfo : SimpleProcessStartInfo
        {
            public TerraformProcessStartInfo() : base(TerraformCommandName)
            {
            }
        }

        internal interface IWorkingDirectoryCloningOptions
        {
            string WorkingDirectory { get; }
        }

        internal class WorkingDirectoryCloning : TerraformCommand, IDisposable, IWorkingDirectoryCloningOptions
        {
            private static WorkingDirectoryCloningOptions ProvideOptions(Action<WorkingDirectoryCloningOptions>? configureOptions, out WorkingDirectoryCloningOptions options)
            {
                options = new WorkingDirectoryCloningOptions();
                configureOptions?.Invoke(options);
                return options;
            }

            public new string WorkingDirectory
            {
                get => base.WorkingDirectory!;
                private init => base.WorkingDirectory = value;
            }

            private readonly WorkingDirectoryCloningOptions _options;
            private bool _isDisposed;

            public WorkingDirectoryCloning(Action<WorkingDirectoryCloningOptions>? configureOptions)
                : base(ProvideOptions(configureOptions, out var options))
            {

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

            internal class WorkingDirectoryCloningOptions : TerraformCommandOptionsBase<WorkingDirectoryCloningOptions>
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
