using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Contains extension methods for <see cref="IProviderContext"/>.
    /// </summary>
    public static class ProviderFeatureDependencyInjectionExtensions
    {
        internal static OptionsBuilder<ProviderOptions> AddProviderOptions(this IServiceCollection services) =>
            services.AddOptions<ProviderOptions>();

        internal static OptionsBuilder<ProviderOptions> AddProviderOptions(this IProviderFeature provider) =>
            provider.Services.AddOptions<ProviderOptions>();

        internal static void SetProvider(this IProviderFeature providerFeature, Type providerType, Type schemaType, object? provider = null) =>
            providerFeature.AddProviderOptions().Configure(options => options.ProviderDescriptor = new ProviderServiceDescriptor(providerType, schemaType) { Implementation = provider });

        public static void SetProvider<Provider, Schema>(this IProviderFeature providerFeature)
            where Provider : class, IProvider<Schema>
            where Schema : class =>
            providerFeature.SetProvider(typeof(Provider), typeof(Schema));

        public static void SetProvider<Provider>(this IProviderFeature providerFeature, Provider? provider = null)
            where Provider : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IProvider
        {
            var providerType = typeof(Provider);
            var schematype = DesignTimeTerraformService.GetSchemaType(providerType);
            providerFeature.SetProvider(providerType, schematype, provider);
        }

        internal static void SetProviderMeta(this IProviderFeature providerFeature, Type schemaType) =>
            providerFeature.AddProviderOptions().Configure(options => options.ProviderSchemaType = schemaType);

        internal static void AddResource<TProviderFeature>(this TProviderFeature providerFeature, Type resourceType, Type schemaType, Type providerMetaSchemaType, object? resource = null)
            where TProviderFeature : IProviderFeature =>
            providerFeature.AddProviderOptions().Configure(options => options.ResourceDescriptors.Add(new ResourceServiceDescriptor(resourceType, schemaType, providerMetaSchemaType) { Implementation = resource }));

        internal static void AddDataSource<TProviderFeature>(this TProviderFeature providerFeature, Type dataSourceType, Type schemaType, Type providerMetaSchemaType, object? dataSource = null)
            where TProviderFeature : IProviderFeature =>
            providerFeature.AddProviderOptions().Configure(options => options.DataSourceDescriptors.Add(new DataSourceServiceDescriptor(dataSourceType, schemaType, providerMetaSchemaType) { Implementation = dataSource }));
    }
}
