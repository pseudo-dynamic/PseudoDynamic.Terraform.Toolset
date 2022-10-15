using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class PluginServiceCollectionExtensions
    {
        public static IServiceCollection AddTerraformPlugin(this IServiceCollection services)
        {
            services.AddGrpc();
            services.TryAddSingleton<IPluginServer, PluginServer>();
            return services;
        }
    }
}
