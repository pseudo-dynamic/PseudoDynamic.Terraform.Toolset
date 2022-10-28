using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Contains extension methods for <see cref="IProvider"/>.
    /// </summary>
    public static class ProviderFeatureDependencyInjectionExtensions
    {
        internal static OptionsBuilder<ProviderOptions> AddProviderOptions(this IProviderFeature provider)
        {
            var serviceProvider = provider.Services;
            return serviceProvider.AddOptions<ProviderOptions>();
        }

        internal static OptionsBuilder<ProviderOptions> ConfigureProviderOptions(this IProviderFeature provider, Action<ProviderOptions> configureOptions) =>
            provider.AddProviderOptions().Configure(configureOptions);

        /// <summary>
        /// Makes the Terraform provider available by enabling gRPC, registering required
        /// services and giving you the chance to register resources and data sources.
        /// <param name="services"></param>
        /// <param name="providerName">Fully-qualified Terraform provider name in form of <![CDATA[<domain-name>/<namespace>/<provider-name>]]> (e.g. registry.terraform.io/pseudo-dynamic/value)</param>
        internal static IServiceCollection AddTerraformProvider(this IServiceCollection services)
        {
            // plugin server
            services.AddTerraformPluginServer();

            // provider
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.TryAddSingleton<TerraformDynamicMessagePackDecoder>();
            services.TryAddSingleton<DynamicDefinitionResolver>();
            services.TryAddSingleton<TerraformDynamicMessagePackEncoder>();
            services.TryAddSingleton<ITerraformDynamicDecoder, TerraformDynamicDecoder>();
            services.TryAddSingleton<IProviderAdapter, ProviderAdapter>();
            services.TryAddSingleton<ResourceDefinitionFactory>();
            services.TryAddSingleton<IProvider, Provider>();
            return services;
        }

        internal static TProviderFeature AddResource<TProviderFeature>(this TProviderFeature providerFeature, Type resourceType, Type schemaType)
            where TProviderFeature : IProviderFeature
        {
            providerFeature.AddProviderOptions().Configure(provider => provider.ResourceDescriptors.Add(new ResourceDescriptor(resourceType, schemaType)));
            return providerFeature;
        }

        internal static TProviderFeature AddResource<TProviderFeature>(this TProviderFeature providerFeature, object resource, Type schemaType)
            where TProviderFeature : IProviderFeature
        {
            providerFeature.AddProviderOptions().Configure(provider => provider.ResourceDescriptors.Add(new ResourceDescriptor(resource, schemaType)));
            return providerFeature;
        }

        public static TProviderFeature ConfigureResources<TProviderFeature>(this TProviderFeature providerFeature, Action<IProviderFeature.IResourceFeature> configureResources)
            where TProviderFeature : IProviderFeature, IProviderFeature.IResourceFeature
        {
            if (configureResources is null) {
                throw new ArgumentNullException(nameof(configureResources));
            }

            configureResources(new IProviderFeature.ResourceFeature(providerFeature));
            return providerFeature;
        }
    }
}
