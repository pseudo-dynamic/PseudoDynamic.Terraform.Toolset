using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class PluginServerDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the necessary plugin server capabilities.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureServer"></param>
        /// <remarks>
        /// <para>
        /// A call to <see cref="ProviderSetupDependencyInjectionExtensions.AddTerraformProvider(IServiceCollection, string)"/> is required.
        /// </para>
        /// <para>
        /// Calls of <see cref="KestrelServerOptions.Listen(System.Net.EndPoint)"/> and similiar must occur AFTER <see cref="AddTerraformPluginServer"/>
        /// because defaults specified through <see cref="KestrelServerOptions.ConfigureEndpointDefaults(Action{ListenOptions})"/> only applies on *newly*
        /// created listen options. Otherwise you need to ensure to use <see cref="HttpProtocols.Http2"/> for these Kestrel server listen options.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddTerraformPluginServer(this IServiceCollection services, Action<PluginServerOptions>? configureServer = null)
        {
            services.AddGrpc();

            if (configureServer != null) {
                services.AddOptions<PluginServerOptions>().Configure(configureServer);
            }

            if (!services.Any(x => x.ImplementationType == typeof(PluginServerOptionsConfiguration))) {
                services.AddSingleton<IConfigureOptions<PluginServerOptions>, PluginServerOptionsConfiguration>();
            }

            services.TryAddSingleton<IPluginServer, PluginServer>();
            return services;
        }

        private class PluginServerOptionsConfiguration : IConfigureOptions<PluginServerOptions>
        {
            public void Configure(PluginServerOptions options)
            {
                if (!options.Protocol.HasValue) {
                    options.Protocol = PluginHostBuilder.DefaultProtocol;
                }
            }
        }
    }
}
