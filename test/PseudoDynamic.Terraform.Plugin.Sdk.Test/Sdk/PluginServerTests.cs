using FluentAssertions;
using Moq;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class PluginServerTests : IClassFixture<PluginHostFixtures.ProtocolV5>
    {
        private readonly PluginHostFixtureBase _pluginHostFixture;

        public PluginServerTests(PluginHostFixtures.ProtocolV5 pluginHostFixture) =>
            _pluginHostFixture = pluginHostFixture;

        [Fact]
        public async Task Plugin_server_survives_terraform_validate()
        {
            var resourceMock = new Mock<IResource<ValidateSchema>>();
            resourceMock.SetupGet(x => x.TypeName).Returns("validate");
            _pluginHostFixture.Provider.ReplaceResourceDefinition(new ResourceDescriptor(resourceMock.Object, typeof(ValidateSchema)));

            var terraform = _pluginHostFixture.CreateTerraformCommand("TerraformProjects/resource_validate");
            Record.Exception(terraform.Validate).Should().BeNull();

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
