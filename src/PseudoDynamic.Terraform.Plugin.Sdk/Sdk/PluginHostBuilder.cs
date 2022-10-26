using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Represents the entry point for hosting a Terraform plugin.
    /// </summary>
    public class PluginHostBuilder : HostBuilder, IPluginHostBuilder
    {
        internal const PluginProtocol DefaultProtocol = PluginProtocol.V6;

        /// <summary>
        /// The intended plugin protocol version to be used by the gRPC server.
        /// </summary>
        public PluginProtocol Protocol { get; init; } = DefaultProtocol;

        /// <summary>
        /// By default, HTTPS is enabled, which is required by Terraform in production environment.
        /// However, if this value is set to <see langword="true"/>, HTTP is used instead and the
        /// instructions for Terraform using this debug instance are displayed in the console output.
        /// </summary>
        public bool IsDebuggable { get; init; }

        /// <inheritdoc/>
        public new IHost Build()
        {
            ConfigureServices((_, services) => services.AddTerraformPluginServer(plugin => {
                plugin.Protocol = Protocol;
                plugin.IsDebuggable = IsDebuggable;
            }));

            return base.Build();
        }

        IHost IHostBuilder.Build() => Build();
    }
}
