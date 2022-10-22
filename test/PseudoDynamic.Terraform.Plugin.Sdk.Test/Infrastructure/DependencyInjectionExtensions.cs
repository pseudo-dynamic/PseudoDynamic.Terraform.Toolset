using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Sdk;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal static class DependencyInjectionExtensions
    {
        public static TerraformCommand.WorkingDirectoryCloning GetWorkingDirectoryCloningTerraformCommand(
            this IServiceProvider serviceProvider,
            Action<TerraformCommand.WorkingDirectoryCloning.WorkingDirectoryCloningOptions>? configureOptions = null)
        {
            var pluginServer = serviceProvider.GetRequiredService<IPluginServer>();
            var provider = serviceProvider.GetRequiredService<IProvider>();

            var pluginProtocol = pluginServer.PluginProtocol;
            var pluginServerHostPort = $"{pluginServer.ServerAddress.Host}:{pluginServer.ServerAddress.Port}";
            var providerName = provider.FullyQualifiedProviderName;

            return new TerraformCommand.WorkingDirectoryCloning(options =>
            {
                options.WithReattachingProvider(providerName, new TerraformReattachProvider(pluginProtocol, new TerraformReattachProviderAddress(pluginServerHostPort)));
                configureOptions?.Invoke(options);
            });
        }

        public static TerraformCommand.WorkingDirectoryCloning GetWorkingDirectoryCloningTerraformCommand(
            this IServiceProvider serviceProvider,
            string? workingDirectory) =>
            GetWorkingDirectoryCloningTerraformCommand(serviceProvider, o => o.WorkingDirectory = workingDirectory);
    }
}
