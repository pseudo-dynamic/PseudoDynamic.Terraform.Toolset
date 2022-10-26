﻿using System.Reflection;
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
    public static class ProviderSetupDependencyInjectionExtensions
    {
        internal static OptionsBuilder<ProviderOptions> AddProviderOptions(this IProviderSetup provider)
        {
            var serviceProvider = provider.Services;
            return serviceProvider.AddOptions<ProviderOptions>();
        }

        internal static OptionsBuilder<ProviderOptions> ConfigureProviderOptions(this IProviderSetup provider, Action<ProviderOptions> configureOptions) =>
            provider.AddProviderOptions().Configure(configureOptions);

        /// <summary>
        /// Makes the Terraform provider available by enabling gRPC, registering required
        /// services and give you the chance to register resources and data sources.
        /// <param name="services"></param>
        /// <param name="providerName">Fully-qualified Terraform provider name in form of <![CDATA[<domain-name>/<namespace>/<provider-name>]]> (e.g. registry.terraform.io/pseudo-dynamic/value)</param>
        public static IProviderSetup AddTerraformProvider(this IServiceCollection services, string providerName)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTerraformPluginServer();
            services.TryAddSingleton<TerraformDynamicMessagePackDecoder>();
            services.TryAddSingleton<IProviderAdapter, ProviderAdapter>();
            services.TryAddSingleton<ResourceDefinitionFactory>();
            services.TryAddSingleton<IProvider, Provider>();

            var providerSetup = new ProviderSetup(services);
            providerSetup.ConfigureProviderOptions(provider => provider.FullyQualifiedProviderName = providerName);
            return providerSetup;
        }

        internal static IProviderSetup AddResource(this IProviderSetup provider, Type resourceType, Type schemaType)
        {
            provider.AddProviderOptions().Configure(provider => provider.ResourceDescriptors.Add(new ResourceDescriptor(resourceType, schemaType)));
            return provider;
        }

        internal static IProviderSetup AddResource(this IProviderSetup provider, object resource, Type schemaType)
        {
            provider.AddProviderOptions().Configure(provider => provider.ResourceDescriptors.Add(new ResourceDescriptor(resource, schemaType)));
            return provider;
        }
    }
}