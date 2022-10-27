using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class PluginServerDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the necessary plugin server capabilities.
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>
        /// <para>
        /// A call to <see cref="ProviderFeatureDependencyInjectionExtensions.AddTerraformProvider(IServiceCollection, string)"/> is required.
        /// </para>
        /// <para>
        /// Calls of <see cref="KestrelServerOptions.Listen(System.Net.EndPoint)"/> and similiar must occur AFTER <see cref="AddTerraformPluginServer"/>
        /// because defaults specified through <see cref="KestrelServerOptions.ConfigureEndpointDefaults(Action{ListenOptions})"/> only applies on *newly*
        /// created listen options. Otherwise you need to ensure to use <see cref="HttpProtocols.Http2"/> for these Kestrel server listen options.
        /// </para>
        /// </remarks>
        internal static IServiceCollection AddTerraformPluginServer(this IServiceCollection services)
        {
            services.AddGrpc();
            services.TryAddSingleton<IPluginServer, PluginServer>();
            return services;
        }

        private static void ConfigureTerraformPluginServer(this IServiceCollection services, IPluginServerSpecification serverSpecification)
        {
            services.AddOptions<PluginServerOptions>().Configure(options => {
                options.Protocol = serverSpecification.Protocol;
                options.IsDebuggable = serverSpecification.IsDebuggable;
            });
        }

        private static void ConfigureTerraformProvider<TProviderFeatures>(
            TProviderFeatures providerFeatures,
            string providerName,
            IEnumerable<Action<TProviderFeatures>> configureProviderDelegates)
            where TProviderFeatures : IProviderFeature
        {
            providerFeatures.ConfigureProviderOptions(options => options.FullyQualifiedProviderName = providerName);

            foreach (var configureProvider in configureProviderDelegates) {
                configureProvider(providerFeatures);
            }
        }

        public static IServiceCollection AddTerraformPluginServer(
            this IServiceCollection services,
            IPluginServerSpecification.ProtocolV5 serverSpecification)
        {
            services.AddTerraformProvider();
            services.ConfigureTerraformPluginServer(serverSpecification);

            if (serverSpecification.ProviderConfigurations.HasValue) {
                ConfigureTerraformProvider(
                    new IPluginServerSpecification.ProtocolV5.ProviderFeatures(services),
                    serverSpecification.ProviderConfigurations.Value.ProviderName,
                    serverSpecification.ProviderConfigurations.Value.Delegates);
            }

            return services;
        }

        public static IServiceCollection AddTerraformPluginServer(
            this IServiceCollection services,
            IPluginServerSpecification.ProtocolV6 serverSpecification)
        {
            services.AddTerraformProvider();
            services.ConfigureTerraformPluginServer(serverSpecification);

            if (serverSpecification.ProviderConfigurations.HasValue) {
                ConfigureTerraformProvider(
                    new IPluginServerSpecification.ProtocolV6.ProviderFeatures(services),
                    serverSpecification.ProviderConfigurations.Value.ProviderName,
                    serverSpecification.ProviderConfigurations.Value.Delegates);
            }

            return services;
        }
    }
}
