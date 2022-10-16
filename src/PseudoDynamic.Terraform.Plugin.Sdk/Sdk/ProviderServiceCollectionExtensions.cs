using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class ProviderServiceCollectionExtensions
    {
        /// <summary>
        /// Makes the Terraform provider available by enabling gRPC, registering required
        /// services and give you the chance to register resources and data sources.
        /// </summary>
        /// <typeparam name="Schema"></typeparam>
        /// <param name="services"></param>
        /// <param name="providerName">Fully-qualified Terraform provider name in form of <![CDATA[<domain-name>/<namespace>/<provider-name>]]> (e.g. registry.terraform.io/pseudo-dynamic/value)</param>
        public static IProviderBuilder AddTerraformProvider<Schema>(this IServiceCollection services, string providerName)
            where Schema : class
        {
            services.AddTerraformPlugin();
            services.TryAddSingleton<IProviderAdapter, ProviderAdapter>();
            services.TryAddSingleton<IProvider, Provider>();

            var providerBuilder = new ProviderBuilder(services);
            providerBuilder.ConfigureProviderOptions(o => o.FullyQualifiedProviderName = providerName);
            return providerBuilder;
        }

        internal static OptionsBuilder<ProviderOptions> AddProviderOptions(this IProviderBuilder provider) =>
            provider.Services.AddOptions<ProviderOptions>();

        internal static OptionsBuilder<ProviderOptions> ConfigureProviderOptions(this IProviderBuilder provider, Action<ProviderOptions> configureOptions) =>
            provider.AddProviderOptions().Configure(configureOptions);

        internal static IProviderBuilder AddResource(this IProviderBuilder provider, Type resourceType, Type schemaType)
        {
            provider.AddProviderOptions().Configure(o => o.ResourceDescriptors.Add(new ResourceDescriptor(resourceType, schemaType)));
            return provider;
        }
    }

    public static class ProviderClassServiceCollectionExtensions
    {
        public static IProviderBuilder AddResource<Resource, Schema>(this IProviderBuilder provider)
            where Resource : IResource<Schema>
            where Schema : class
        {
            provider.AddResource(typeof(Resource), typeof(Schema));
            return provider;
        }
    }

    public static class ProviderStructServiceCollectionExtensions
    {
        public static IProviderBuilder AddResource<Resource, Schema>(this IProviderBuilder provider)
            where Resource : IResource<Schema>
            where Schema : struct
        {
            provider.AddResource(typeof(Resource), typeof(Schema));
            return provider;
        }
    }
}
