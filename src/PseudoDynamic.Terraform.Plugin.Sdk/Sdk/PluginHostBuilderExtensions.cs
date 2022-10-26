using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk.Kestrel;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class PluginHostBuilderExtensions
    {
        /// <summary>
        /// Applies Terraform provider defaults.
        /// </summary>
        /// <param name="pluginServer"></param>
        /// <param name="providerName"></param>
        /// <param name="setupProvider"></param>
        public static IPluginHostBuilder ConfigureTerraformProviderDefaults(this IPluginHostBuilder pluginServer, string providerName, Action<IProviderSetup>? setupProvider = null)
        {
            pluginServer.ConfigureWebHost(builder => builder
                    .UseKestrel()
                    .ConfigureServices(services => {
                        services.AddKestrelLoopbackListener();
                        var providerSetup = services.AddTerraformProvider(providerName);
                        setupProvider?.Invoke(providerSetup);
                    })
                    .Configure(app => {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapTerraformPlugin());
                    }));

            return pluginServer;
        }
    }
}
