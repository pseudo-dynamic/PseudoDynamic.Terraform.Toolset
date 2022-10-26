using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Protocols.V5;
using PseudoDynamic.Terraform.Plugin.Protocols.V6;
using PseudoDynamic.Terraform.Plugin.Sdk;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal static class PluginServerEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Tells gRPC what protocol-specific implementation needs to be called. The protocol version
        /// you specified previously by <see cref="ProviderSetupDependencyInjectionExtensions.AddTerraformProvider(IServiceCollection, string, PluginProtocol)"/>
        /// is used.
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="NotSupportedException"></exception>
        public static IEndpointRouteBuilder MapTerraformPlugin(this IEndpointRouteBuilder builder)
        {
            var pluginServer = builder.ServiceProvider.GetRequiredService<IPluginServer>();
            var pluginProtocol = pluginServer.PluginProtocol;

            if (pluginProtocol == PluginProtocol.V5) {
                builder.MapTerraformPluginProtocolV5();
            } else if (pluginProtocol == PluginProtocol.V6) {
                builder.MapTerraformPluginProtocolV6();
            } else {
                throw new NotSupportedException("Bad protocol");
            }

            return builder;
        }
    }
}
