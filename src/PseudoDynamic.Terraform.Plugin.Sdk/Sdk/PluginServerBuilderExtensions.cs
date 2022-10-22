using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class PluginServerBuilderExtensions
    {
        public static IPluginHostBuilder ConfigureTerraformProviderDefaults(this IPluginHostBuilder pluginServer, string providerName, Action<IProviderSetup>? setupProvider = null)
        {
            pluginServer.ConfigureWebHost(builder => builder
                    .UseUrls("http://127.0.0.1:0")
                    .UseKestrel()
                    .ConfigureServices(services => {
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
