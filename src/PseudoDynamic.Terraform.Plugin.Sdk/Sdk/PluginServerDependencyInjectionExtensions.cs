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
        /// A call to <see cref="ProviderFeatureServiceCollectionExtensions.AddTerraformProvider(IServiceCollection, string)"/> is required.
        /// </para>
        /// <para>
        /// Calls of <see cref="KestrelServerOptions.Listen(System.Net.EndPoint)"/> and similiar must occur AFTER <see cref="AddTerraformPluginServer"/>
        /// because defaults specified through <see cref="KestrelServerOptions.ConfigureEndpointDefaults(Action{ListenOptions})"/> only applies on *newly*
        /// created listen options. Otherwise you need to ensure to use <see cref="HttpProtocols.Http2"/> for these Kestrel server listen options.
        /// </para>
        /// </remarks>
        internal static IServiceCollection AddTerraformPluginServer(this IServiceCollection services)
        {
            services.AddGrpc(x => x.IgnoreUnknownServices = true);
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

        private static void ConfigureTerraformProvider<TServerSpecification>(
            IServiceCollection services,
            PluginServerSpecificationBase<TServerSpecification> serverSpecification)
            where TServerSpecification : PluginServerSpecificationBase<TServerSpecification>
        {
            if (!serverSpecification.ProviderConfiguration.HasValue) {
                return;
            }

            services.AddProviderOptions().Configure(options => {
                options.FullyQualifiedProviderName = serverSpecification.ProviderConfiguration.Value.ProviderName;
                options.ProviderSchemaType = serverSpecification.ProviderConfiguration.Value.ProviderMetaSchemaType;
            });

            serverSpecification.ProviderConfiguration.Value.ConfigureProvider(services);
        }

        public static IServiceCollection AddTerraformPluginServer(
            this IServiceCollection services,
            IPluginServerSpecification.ProtocolV5 serverSpecification)
        {
            services.AddTerraformProvider();
            services.ConfigureTerraformPluginServer(serverSpecification);
            ConfigureTerraformProvider(services, serverSpecification);
            return services;
        }

        public static IServiceCollection AddTerraformPluginServer(
            this IServiceCollection services,
            IPluginServerSpecification.ProtocolV6 serverSpecification)
        {
            services.AddTerraformProvider();
            services.ConfigureTerraformPluginServer(serverSpecification);
            ConfigureTerraformProvider(services, serverSpecification);
            return services;
        }
    }
}
