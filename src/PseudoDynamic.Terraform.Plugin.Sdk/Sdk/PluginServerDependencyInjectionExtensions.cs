using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class PluginServerDependencyInjectionExtensions
    {
        internal static IServiceCollection AddTerraformPluginServer(this IServiceCollection services)
        {
            services.AddGrpc();
            services.TryAddSingleton<IPluginServer, PluginServer>();

            if (!services.Any(x => x.ImplementationType == typeof(KestrelServerOptionsConfiguration))) {
                services.AddSingleton<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsConfiguration>();
            }

            return services;
        }

        private class KestrelServerOptionsConfiguration : IConfigureOptions<KestrelServerOptions>
        {
            public void Configure(KestrelServerOptions options) =>
                options.ConfigureEndpointDefaults(endpoints => endpoints.Protocols = HttpProtocols.Http2);
        }
    }
}
