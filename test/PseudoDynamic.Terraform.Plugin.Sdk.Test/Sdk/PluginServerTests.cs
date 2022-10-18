using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Schema;
using Shouldly;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class PluginServerTests
    {
        [Fact]
        public async Task Plugin_server_survives_terraform_validate()
        {
            var providerName = "registry.terraform.io/pseudo-dynamic/debug";

            var resourceMock = new Mock<IResource<ValidateSchema>>();
            resourceMock.SetupGet(x => x.TypeName).Returns("validate").Verifiable();
            //resourceMock.Setup(x => x.ValidateConfig()).Verifiable();

            using var host = await new HostBuilder()
                .ConfigureWebHostDefaults(builder => builder
                    .UseUrls("http://127.0.0.1:0")
                    .UseKestrel(options => options.ConfigureEndpointDefaults(endpoints => endpoints.Protocols = HttpProtocols.Http2))
                    .ConfigureServices(services => services
                        .AddTerraformProvider(providerName)
                            .AddResource(resourceMock.Object, typeof(ValidateSchema))
                        .Services
                        .AddAutoMapper(typeof(PluginServerTests).Assembly))
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapTerraformPlugin(PluginProtocol.V5));
                    }))
                .StartAsync();

            var pluginServer = host.Services.GetRequiredService<IPluginServer>();
            var serverAddress = $"{pluginServer.ServerAddress.Host}:{pluginServer.ServerAddress.Port}";

            using var terraformCommand = new TerraformCommand.WorkingDirectoryCloning("TerraformProjects/resource_empty")
            {
                TerraformReattachProviders = { { providerName, new TerraformReattachProvider(PluginProtocol.V5, new TerraformReattachProviderAddress(serverAddress)) } }
            };

            Record.Exception(terraformCommand.Init).ShouldBeNull();
            Record.Exception(terraformCommand.Validate).ShouldBeNull();

            resourceMock.VerifyAll();
            resourceMock.VerifyNoOtherCalls();
        }

        [Block]
        public class ValidateSchema
        {
        }
    }
}
