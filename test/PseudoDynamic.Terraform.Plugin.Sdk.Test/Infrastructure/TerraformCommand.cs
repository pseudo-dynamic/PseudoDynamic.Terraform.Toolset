﻿using PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics;
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

        private const string TfReattachProvidersVariableName = "TF_REATTACH_PROVIDERS";
        private const string TfCliConfigFileVariableName = "TF_CLI_CONFIG_FILE";
        private const string TerraformCommandName = "terraform";

        public string? WorkingDirectory { get; init; }
        public IReadOnlyDictionary<string, string> EnvironmentVariables { get; }
        public IReadOnlyDictionary<string, TerraformReattachProvider>? TerraformReattachProviders { get; }
        public string TfCliConfigFile { get; }

        private TerraformProcessStartInfo? _preparedProcessStartInfo;

        internal TerraformCommand(ITerraformCommandOptionsOptions options)
        {
            TerraformReattachProviders = new Dictionary<string, TerraformReattachProvider>(options.TerraformReattachProviders);
            EnvironmentVariables = new Dictionary<string, string>(options.EnvironmentVariables);
            TfCliConfigFile = options.TfCliConfigFile;
        }

        public TerraformCommand(Action<TerraformCommandOptions> configureOptions)
            : this(ProvideOptions(configureOptions))
        {
        }

        private TerraformProcessStartInfo PrepareStartInfo()
        {
            var processStartInfo = _preparedProcessStartInfo;

            if (processStartInfo != null)
            {
                return processStartInfo;
            }

            var environmentVariables = new Dictionary<string, string>(EnvironmentVariables);

            if (TerraformReattachProviders != null && TerraformReattachProviders.Count > 0)
            {
                environmentVariables.Add(TfReattachProvidersVariableName, JsonSerializer.Serialize(TerraformReattachProviders));
            }

            if (TfCliConfigFile != null)
            {
                environmentVariables.Add(TfCliConfigFileVariableName, TfCliConfigFile);
            }

            processStartInfo = new TerraformProcessStartInfo()
            {
                WorkingDirectory = WorkingDirectory,
                EnvironmentVariables = environmentVariables
            };

            _preparedProcessStartInfo = processStartInfo;
            return processStartInfo;
        }

        private TerraformProcessStartInfo UpgradeStartInfo(string? args) => PrepareStartInfo() with
        {
            Arguments = args
        };

        private string RunCommandThenReadOutput(string? args) => SimpleProcess.StartThenWaitForExitThenReadOutput(
            UpgradeStartInfo(args),
            shouldThrowOnNonZeroCode: true,
            shouldStreamError: true);

        private Task<string> RunCommandThenReadOutputAsync(string? args, CancellationToken cancellationToken) => SimpleProcess.StartThenWaitForExitThenReadOutputAsync(
            UpgradeStartInfo(args),
            shouldThrowOnNonZeroCode: true,
            cancellationToken: cancellationToken);

        public string Init() => RunCommandThenReadOutput("init");

        public string Validate() => RunCommandThenReadOutput("validate");

        //public Task<string> ValidateAsync(CancellationToken cancellationToken = default) => RunCommandThenReadOutputAsync("validate", cancellationToken);

        public string Plan() => RunCommandThenReadOutput("plan -input=false");

        public string Apply() => RunCommandThenReadOutput("apply -input=false -auto-approve");

        internal interface ITerraformCommandOptionsOptions
        {
            IEnumerable<KeyValuePair<string, string>> EnvironmentVariables { get; }
            IEnumerable<KeyValuePair<string, TerraformReattachProvider>> TerraformReattachProviders { get; }
            string TfCliConfigFile { get; }
        }

        public class TerraformCommandOptionsBase<DerivedOptions> : ITerraformCommandOptionsOptions
            where DerivedOptions : TerraformCommandOptionsBase<DerivedOptions>
        {
            public IDictionary<string, string> EnvironmentVariables
            {
                get => _environmentVariables;
                set => _environmentVariables = value ?? throw new ArgumentNullException(nameof(value));
            }

            IEnumerable<KeyValuePair<string, string>> ITerraformCommandOptionsOptions.EnvironmentVariables => EnvironmentVariables;

            public IDictionary<string, TerraformReattachProvider> TerraformReattachProviders
            {
                get => _terraformReattachProviders;
                set => _terraformReattachProviders = value ?? throw new ArgumentNullException(nameof(value));
            }

            IEnumerable<KeyValuePair<string, TerraformReattachProvider>> ITerraformCommandOptionsOptions.TerraformReattachProviders => TerraformReattachProviders;

            /// <summary>
            /// A file path, specifying a configuration file as it is described here:
            /// <see href="https://developer.hashicorp.com/terraform/cli/config/config-file">https://developer.hashicorp.com/terraform/cli/config/config-file</see>.
            /// </summary>
            /// <remarks>
            /// Use it to abbreviate the setting of the corresponding environment variable, namely <b>TF_CLI_CONFIG_FILE</b>.
            /// </remarks>
            public string? TfCliConfigFile { get; set; }

            private IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();
            private IDictionary<string, TerraformReattachProvider> _terraformReattachProviders = new Dictionary<string, TerraformReattachProvider>();

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

            public string OriginalWorkingDirectory { get; }

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

                var workingDirectoryInfo = new DirectoryInfo(workingDirectory);
                workingDirectory = workingDirectoryInfo.FullName;
                OriginalWorkingDirectory = workingDirectory;
                Directory.CreateDirectory(workingDirectory);
                var copyableFilePatterns = options.CopyableFilePatterns;

                foreach (var file in copyableFilePatterns.AsParallel().SelectMany(filePattern => workingDirectoryInfo.EnumerateFiles(filePattern, SearchOption.AllDirectories)))
                {
                    var relativeDirectory = file.DirectoryName!.Remove(0, workingDirectory.Length);
                    var mirroringDirectory = options.TemporaryWorkingDirectory + relativeDirectory;
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
                private static readonly string[] TerraformFilePattern = new[] { "*.tf" };

                public string? WorkingDirectory
                {
                    get => workingDirectory ?? AppContext.BaseDirectory;
                    set => workingDirectory = value;
                }

                public string[] CopyableFilePatterns
                {
                    get => copyableFilePatterns;
                    set => copyableFilePatterns = value ?? throw new ArgumentNullException(nameof(value));
                }

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

                private string? workingDirectory;
                private string[] copyableFilePatterns = TerraformFilePattern;

                public WorkingDirectoryCloningOptions() => TemporaryWorkingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                public WorkingDirectoryCloningOptions SetCopyableFilePatterns(params string[] copyableFilePatterns)
                {
                    CopyableFilePatterns = copyableFilePatterns;
                    return this;
                }
            }
        }
    }
}
