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
            var providerContext = serviceProvider.GetRequiredService<IProviderServer>();
            var providerName = providerContext.FullyQualifiedProviderName;

            return new TerraformCommand.WorkingDirectoryCloning(options => {
                options.WithReattachingProvider(providerName, providerContext.TerraformReattachProvider);
                configureOptions?.Invoke(options);
            });
        }

        public static TerraformCommand.WorkingDirectoryCloning GetWorkingDirectoryCloningTerraformCommand(
            this IServiceProvider serviceProvider,
            string? workingDirectory) =>
            GetWorkingDirectoryCloningTerraformCommand(serviceProvider, o => o.WorkingDirectory = workingDirectory);
    }
}
