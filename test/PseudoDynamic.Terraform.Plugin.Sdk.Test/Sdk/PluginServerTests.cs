using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Moq;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class PluginServerTests
    {
        [Fact]
        public async Task Plugin_server_survives_terraform_validate()
        {
            var resourceMock = new Mock<IResource<ValidateSchema>>();
            resourceMock.SetupGet(x => x.TypeName).Returns("validate");

            using var host = await new PluginHostBuilder() { Protocol = PluginProtocol.V5 }
                .ConfigureTerraformProviderDefaults("registry.terraform.io/pseudo-dynamic/debug", provider => provider
                    .AddResource(resourceMock.Object, typeof(ValidateSchema)))
                .StartAsync();

            var terraformCommand = host.Services.GetWorkingDirectoryCloningTerraformCommand("TerraformProjects/resource_validate");

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
