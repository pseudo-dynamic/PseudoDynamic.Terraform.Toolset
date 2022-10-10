using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class PluginServerDependencyInjectionExtensions
    {
        public static IServiceCollection AddTerraformPlugin(this IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<IPluginServer, PluginServer>();
            return services;
        }
    }
}
