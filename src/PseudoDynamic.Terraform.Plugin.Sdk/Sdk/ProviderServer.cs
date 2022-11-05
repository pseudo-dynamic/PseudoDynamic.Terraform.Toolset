using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Conventions;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using static PseudoDynamic.Terraform.Plugin.Infrastructure.Terraform;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderServer : IProviderServer
    {
        public string FullyQualifiedProviderName => _lazyFullyQualifiedProviderName.Value;

        /// <summary>
        /// The provider name. Represents the last "/"-separated part of <see cref="FullyQualifiedProviderName"/>.
        /// </summary>
        public string ProviderName => _providerName ??= FullyQualifiedProviderName.Split("/").Last();

        /// <summary>
        /// Same as <see cref="ProviderName"/> but snake_case formatted to comply with resource and data source naming conventions.
        /// </summary>
        public string SnakeCaseProviderName => _snakeCaseProviderName ??= SnakeCaseConvention.Default.Format(ProviderName);

        internal TerraformReattachProvider TerraformReattachProvider =>
            _terraformReattachProvider ??= new TerraformReattachProvider(_pluginServer.PluginProtocol, new TerraformReattachProviderAddress($"{_pluginServer.ServerAddress.Host}:{_pluginServer.ServerAddress.Port}"));

        TerraformReattachProvider IProviderServer.TerraformReattachProvider => TerraformReattachProvider;

        private readonly IPluginServer _pluginServer;
        private readonly ILogger<ProviderContext> _logger;
        // We must delay validation exception until first incoming gRPC
        private Lazy<string> _lazyFullyQualifiedProviderName;
        private string? _providerName;
        private string? _snakeCaseProviderName;
        private TerraformReattachProvider? _terraformReattachProvider;

        public ProviderServer(IPluginServer pluginServer, IOptions<ProviderOptions> options, ILogger<ProviderContext> logger)
        {
            _pluginServer = pluginServer;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var unwrappedOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _lazyFullyQualifiedProviderName = new(() => unwrappedOptions.FullyQualifiedProviderName);
            pluginServer.ServerStarted.Register(OnPluginServerStarted);
        }

        private void WriteTerraformDebugInstructions()
        {
            var serializedTerraformReattachProviders = SerializeTerraformReattachProviders(new Dictionary<string, TerraformReattachProvider>() {
                { FullyQualifiedProviderName, TerraformReattachProvider }
            });

            _logger.LogInformation($"The plugin server started in debug mode. To make Terraform now use of this running instance, you must set the following environment variable:{Environment.NewLine}"
                + $"Bash: {TfReattachProvidersVariableName}=$'{serializedTerraformReattachProviders.Replace("'", "\\'", StringComparison.InvariantCulture)}'{Environment.NewLine}"
                + $"PowerShell: $env:{TfReattachProvidersVariableName}='{serializedTerraformReattachProviders.Replace("'", "''", StringComparison.InvariantCulture)}'{Environment.NewLine}"
                + $"CMD: set {TfReattachProvidersVariableName}=\"{serializedTerraformReattachProviders.Replace("\"", "\\\"", StringComparison.InvariantCulture)}\"");
        }

        private void OnPluginServerStarted()
        {
            if (_pluginServer.IsDebuggable) {
                WriteTerraformDebugInstructions();
            }
        }
    }
}
