using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Internals;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class PluginServerTests
    {
        [Fact]
        public async Task Test()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHostDefaults(builder => builder
                    .UseUrls("http://127.0.0.1:0")
                    .UseKestrel(options => options.ConfigureEndpointDefaults(endpoints => endpoints.Protocols = HttpProtocols.Http2))
                    .ConfigureServices(services => services.AddTerraformPlugin())
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapTerraformPluginServer());
                    }))
                .StartAsync();

            var pluginServer = host.Services.GetRequiredService<IPluginServer>();
            var serverAddress = $"{pluginServer.ServerAddress.Host}:{pluginServer.ServerAddress.Port}";
            var providerName = "registry.terraform.io/pseudo-dynamic/debug";

            var terraformCommand = new TerraformCommand()
            {
                WorkingDirectory = "TerraformProjects/Init",
                EnvironmentVariables = new Dictionary<string, string>() {
                    {
                        TerraformReattachProviders.TerraformReattachProvidersEnvironmentVariableName,
                        new TerraformReattachProviders() {
                            {
                                providerName,
                                new TerraformReattachProvider() {
                                    Addr = new TerraformReattachProviderAddress(serverAddress)
                                }
                            }
                        }.ToJson()
                    }
                }
            };

            var init = terraformCommand.Init();
            var apply = terraformCommand.Apply();
        }
    }
}
