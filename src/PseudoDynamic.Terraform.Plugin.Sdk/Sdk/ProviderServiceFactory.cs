using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderServiceFactory : TerraformServiceFactory
    {
        public ProviderServiceFactory(IServiceProvider serviceProvider, DynamicDefinitionResolver dynamicResolver)
            : base(serviceProvider, dynamicResolver)
        {
        }

        public ProviderService Build(ProviderServiceDescriptor resourceDescriptor) =>
            new ProviderService(
                BuildSchema(resourceDescriptor),
                BuildService<object>(resourceDescriptor));
    }
}
