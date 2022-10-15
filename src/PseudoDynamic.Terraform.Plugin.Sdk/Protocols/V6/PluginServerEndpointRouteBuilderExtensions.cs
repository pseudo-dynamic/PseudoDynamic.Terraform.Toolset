using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V6
{
    internal static class PluginServerEndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapTerraformPluginProtocolV6<T>(this IEndpointRouteBuilder builder)
            where T : Provider.ProviderBase
        {
            builder.MapGrpcService<T>();
            return builder;
        }

        public static IEndpointRouteBuilder MapTerraformPluginProtocolV6(this IEndpointRouteBuilder builder) =>
            builder.MapTerraformPluginProtocolV6<ProviderAdapter>();
    }
}
