using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Internals;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Protocols.Models;
using PseudoDynamic.Terraform.Plugin.Protocols.V5;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class PluginServerTests
    {
        [Fact]
        public async Task Plugin_server_survives_terraform_validate()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHostDefaults(builder => builder
                    .UseUrls("http://127.0.0.1:0")
                    .UseKestrel(options => options.ConfigureEndpointDefaults(endpoints => endpoints.Protocols = HttpProtocols.Http2))
                    .ConfigureServices(services => services.AddTerraformPlugin())
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapTerraformPlugin(PluginProtocol.V6));
                    }))
                .StartAsync();

            var pluginServer = host.Services.GetRequiredService<IPluginServer>();
            var serverAddress = $"{pluginServer.ServerAddress.Host}:{pluginServer.ServerAddress.Port}";
            var providerName = "registry.terraform.io/pseudo-dynamic/debug";

            var terraformCommand = new TerraformCommand()
            {
                WorkingDirectory = "TerraformProjects/resource_empty",
                TerraformReattachProviders = {
                    { providerName, new TerraformReattachProvider(new TerraformReattachProviderAddress(serverAddress)) }
                }
            };

            //var init = terraformCommand.Init();
            //var apply = terraformCommand.Plan();
        }

        //private class ValidatingProviderAdapter : Protocols.V5.Provider.ProviderBase
        //{
        //    public override Task<GetProviderSchema.Types.Response> GetSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
        //    {
        //        return base.GetSchema(request, context);

        //        //new MapperConfiguration(config => config.AddProfile<>

        //        //return new GetProviderSchema.Types.Response() { 
        //        //    ResourceSchemas = new Google.Protobuf.Collections.MapField<string, Protocols.V5.Schema>() { 

        //        //    }
        //        //}
        //    }

        //    public override Task<ValidateResourceTypeConfig.Types.Response> ValidateResourceTypeConfig(ValidateResourceTypeConfig.Types.Request request, ServerCallContext context)
        //    {
        //        return base.ValidateResourceTypeConfig(request, context);
        //    }
        //}
    }
}
