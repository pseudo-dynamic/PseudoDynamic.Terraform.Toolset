using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal static class PluginServerEndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapTerraformPluginProtocolV5<T>(this IEndpointRouteBuilder builder)
            where T : Provider.ProviderBase
        {
            builder.MapGrpcService<T>();
            return builder;
        }

        public static IEndpointRouteBuilder MapTerraformPluginProtocolV5(this IEndpointRouteBuilder builder) =>
            builder.MapTerraformPluginProtocolV5<ProviderAdapter>();
    }
}
