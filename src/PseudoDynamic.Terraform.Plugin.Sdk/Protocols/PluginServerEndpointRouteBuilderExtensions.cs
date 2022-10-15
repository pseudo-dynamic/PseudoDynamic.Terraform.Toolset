using Microsoft.AspNetCore.Routing;
using PseudoDynamic.Terraform.Plugin.Protocols.V5;
using PseudoDynamic.Terraform.Plugin.Protocols.V6;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal static class PluginServerEndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapTerraformPlugin(this IEndpointRouteBuilder builder, PluginProtocol protocol)
        {
            if (protocol == PluginProtocol.V5) {
                builder.MapTerraformPluginProtocolV5();
            } else if (protocol == PluginProtocol.V6) {
                builder.MapTerraformPluginProtocolV6();
            }

            return builder;
        }
    }
}
