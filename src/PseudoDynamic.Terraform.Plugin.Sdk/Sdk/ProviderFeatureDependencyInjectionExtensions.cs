using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Contains extension methods for <see cref="IProviderContext"/>.
    /// </summary>
    public static class ProviderFeatureDependencyInjectionExtensions
    {
        internal static OptionsBuilder<ProviderContextOptions> AddProviderOptions(this IProviderFeature provider)
        {
            var serviceProvider = provider.Services;
            return serviceProvider.AddOptions<ProviderContextOptions>();
        }

        internal static OptionsBuilder<ProviderContextOptions> ConfigureProviderOptions(this IProviderFeature provider, Action<ProviderContextOptions> configureOptions) =>
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
            services.TryAddSingleton<ProviderServiceFactory>();
            services.TryAddSingleton<ResourceServiceFactory>();
            services.TryAddSingleton<ProviderResourceServiceRegistry>();
            services.TryAddSingleton<DataSourceServiceFactory>();
            services.TryAddSingleton<ProviderDataSourceService>();
            services.TryAddSingleton<ProviderDataSourceServiceRegistry>();
            services.TryAddSingleton<IProviderContext, ProviderContext>();
            services.TryAddSingleton(sp => new Lazy<IProviderContext>(sp.GetRequiredService<IProviderContext>));
            return services;
        }

        internal static TProviderFeature SetProvider<TProviderFeature>(this TProviderFeature providerFeature, Type providerType, Type schemaType, object? provider = null)
            where TProviderFeature : IProviderFeature
        {
            providerFeature.AddProviderOptions().Configure(options => options.ProviderDescriptor = new ProviderServiceDescriptor(providerType, schemaType) { Service = provider });
            return providerFeature;
        }

        public static IProviderFeature SetProvider<Resource, Schema>(this IProviderFeature providerFeature)
            where Resource : class, IProvider<Schema>
            where Schema : class
        {
            providerFeature.SetProvider(typeof(Resource), typeof(Schema));
            return providerFeature;
        }

        public static IProviderFeature SetProvider<Provider>(this IProviderFeature providerFeature, Provider? provider = null)
            where Provider : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IProvider
        {
            var providerType = typeof(Provider);
            var schematype = DesignTimeTerraformService.GetSchemaType(providerType);
            providerFeature.SetProvider(providerType, schematype, provider);
            return providerFeature;
        }

        #region Resource

        internal static TProviderFeature AddResource<TProviderFeature>(this TProviderFeature providerFeature, Type resourceType, Type schemaType, object? resource = null)
            where TProviderFeature : IProviderFeature
        {
            providerFeature.AddProviderOptions().Configure(options => options.ResourceDescriptors.Add(new ResourceServiceDescriptor(resourceType, schemaType) { Service = resource }));
            return providerFeature;
        }

        public static IProviderFeature.IResourceFeature AddResource<Resource, Schema>(this IProviderFeature.IResourceFeature resourceFeature)
            where Resource : class, IResource<Schema>
            where Schema : class
        {
            resourceFeature.ProviderFeature.AddResource(typeof(Resource), typeof(Schema));
            return resourceFeature;
        }

        public static IProviderFeature.IResourceFeature AddResource<Resource, Schema>(this IProviderFeature.IResourceFeature resourceFeature, Resource resource)
            where Resource : class, IResource<Schema>
            where Schema : class
        {
            resourceFeature.ProviderFeature.AddResource(typeof(Resource), typeof(Schema), resource);
            return resourceFeature;
        }

        public static IProviderFeature.IResourceFeature AddResource<Resource>(this IProviderFeature.IResourceFeature resourceFeature, Resource? resource = null)
            where Resource : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IResource
        {
            var resourceType = typeof(Resource);
            var schematype = DesignTimeTerraformService.GetSchemaType(resourceType);
            resourceFeature.ProviderFeature.AddResource(resourceType, schematype, resource);
            return resourceFeature;
        }

        #endregion

        #region DataSource

        internal static TProviderFeature AddDataSource<TProviderFeature>(this TProviderFeature providerFeature, Type dataSourceType, Type schemaType, object? dataSource = null)
            where TProviderFeature : IProviderFeature
        {
            providerFeature.AddProviderOptions().Configure(options => options.DataSourceDescriptors.Add(new DataSourceServiceDescriptor(dataSourceType, schemaType) { Service = dataSource }));
            return providerFeature;
        }

        public static IProviderFeature.IDataSourceFeature AddDataSource<DataSource, Schema>(this IProviderFeature.IDataSourceFeature dataSourceFeature)
            where DataSource : class, IDataSource<Schema>
            where Schema : class
        {
            dataSourceFeature.ProviderFeature.AddDataSource(typeof(DataSource), typeof(Schema));
            return dataSourceFeature;
        }

        public static IProviderFeature.IDataSourceFeature AddDataSource<DataSource, Schema>(this IProviderFeature.IDataSourceFeature dataSourceFeature, DataSource dataSource)
            where DataSource : class, IDataSource<Schema>
            where Schema : class
        {
            dataSourceFeature.ProviderFeature.AddDataSource(typeof(DataSource), typeof(Schema), dataSource);
            return dataSourceFeature;
        }

        public static IProviderFeature.IDataSourceFeature AddDataSource<DataSource>(this IProviderFeature.IDataSourceFeature dataSourceFeature, DataSource? dataSource = null)
            where DataSource : DesignTimeTerraformService, IDesignTimeTerraformService<object>.IDataSource
        {
            var dataSourceType = typeof(DataSource);
            var schematype = DesignTimeTerraformService.GetSchemaType(dataSourceType);
            dataSourceFeature.ProviderFeature.AddDataSource(dataSourceType, schematype, dataSource);
            return dataSourceFeature;
        }

        #endregion
    }
}
