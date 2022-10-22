using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class PluginDependencyInjectionExtensions
    {
        internal static IServiceCollection AddTerraformPlugin(this IServiceCollection services, PluginProtocol protocol = PluginProtocol.V6)
        {
            if (!Enum.IsDefined(protocol)) {
                throw new ArgumentException($"Bad plugin protocol: {protocol}");
            }

            services.AddGrpc();
            services.TryAddSingleton<IPluginServer, PluginServer>();
            services.AddOptions<PluginOptions>().Configure(o => o.Protocol = protocol);
            return services;
        }
    }
}
