using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Contains extension methods for <see cref="IProvider"/>.
    /// </summary>
    public static class ProviderDependencyInjectionExtensions
    {
        internal static OptionsBuilder<ProviderOptions> AddProviderOptions(this IProviderSetup provider)
        {
            var serviceProvider = provider.Services;
            var optionsBuilder = serviceProvider.AddOptions<ProviderOptions>();
            serviceProvider.TryAddSingleton<IPostConfigureOptions<ProviderOptions>, ProviderOptions.RequestResources>();
            return optionsBuilder;
        }

        internal static OptionsBuilder<ProviderOptions> ConfigureProviderOptions(this IProviderSetup provider, Action<ProviderOptions> configureOptions) =>
            provider.AddProviderOptions().Configure(configureOptions);

        /// <summary>
        /// Makes the Terraform provider available by enabling gRPC, registering required
        /// services and give you the chance to register resources and data sources.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="providerName">Fully-qualified Terraform provider name in form of <![CDATA[<domain-name>/<namespace>/<provider-name>]]> (e.g. registry.terraform.io/pseudo-dynamic/value)</param>
        /// <param name="pluginProtocol">Overwrites the default or already provided plugin protocol</param>
        public static IProviderSetup AddTerraformProvider(this IServiceCollection services, string providerName)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTerraformPluginServer();
            services.TryAddSingleton<IProviderAdapter, ProviderAdapter>();
            services.TryAddSingleton<IProvider, Provider>();

            var providerSetup = new ProviderSetup(services);
            providerSetup.ConfigureProviderOptions(o => o.FullyQualifiedProviderName = providerName);
            return providerSetup;
        }

        internal static IProviderSetup AddResource(this IProviderSetup provider, Type resourceType, Type schemaType)
        {
            provider.AddProviderOptions().Configure(o => o.ResourceDescriptors.Add(new ResourceDescriptor(resourceType, schemaType)));
            return provider;
        }

        internal static IProviderSetup AddResource(this IProviderSetup provider, object resource, Type schemaType)
        {
            provider.AddProviderOptions().Configure(o => o.ResourceDescriptors.Add(new ResourceDescriptor(resource, schemaType)));
            return provider;
        }
    }
}
