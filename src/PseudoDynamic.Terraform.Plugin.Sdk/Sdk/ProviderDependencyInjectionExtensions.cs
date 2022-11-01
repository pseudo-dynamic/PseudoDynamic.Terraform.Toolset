using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PseudoDynamic.Terraform.Plugin.Sdk.Services;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class ProviderDependencyInjectionExtensions
    {
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
            services.TryAddSingleton<SchemaBuilder>();
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
    }
}
