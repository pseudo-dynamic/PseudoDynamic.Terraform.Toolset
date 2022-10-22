using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class PluginServerTests : XunitContextBase
    {
        public PluginServerTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Plugin_server_survives_terraform_validate()
        {
            var providerName = "registry.terraform.io/pseudo-dynamic/debug";

            var resourceMock = new Mock<IResource<ValidateSchema>>();
            resourceMock.SetupGet(x => x.TypeName).Returns("validate");

            using var host = await new HostBuilder()
                .ConfigureWebHostDefaults(builder => builder
                    .UseUrls("http://127.0.0.1:0")
                    .UseKestrel(options => options.ConfigureEndpointDefaults(endpoints => endpoints.Protocols = HttpProtocols.Http2))
                    .ConfigureServices(services => services
                        .AddTerraformProvider(providerName, PluginProtocol.V5)
                            .AddResource(resourceMock.Object, typeof(ValidateSchema)))
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapTerraformPlugin());
                    }))
                .StartAsync();

            var test = host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses;

            var pluginServer = host.Services.GetRequiredService<IPluginServer>();
            var serverAddress = $"{pluginServer.ServerAddress.Host}:{pluginServer.ServerAddress.Port}";

            using var terraformCommand = new TerraformCommand.WorkingDirectoryCloning(options =>
            {
                options.WorkingDirectory = "TerraformProjects/resource_validate";
                options.WithReattachingProvider(providerName, new TerraformReattachProvider(PluginProtocol.V5, new TerraformReattachProviderAddress(serverAddress)));
            });

            Record.Exception(terraformCommand.Init).Should().BeNull();
            Record.Exception(terraformCommand.Validate).Should().BeNull();

            resourceMock.Verify(x => x.ValidateConfig(It.Is<ValidateConfig.Context<ValidateSchema>>(x => x.Config.Greeting.Value == "Hello from Terraform!")));
            resourceMock.VerifyAll();
            resourceMock.VerifyNoOtherCalls();
        }

        [Block]
        public class ValidateSchema
        {
            public ITerraformValue<string> Greeting { get; }

            public ValidateSchema(ITerraformValue<string> greeting) =>
                Greeting = greeting;
        }
    }
}
