using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ResourceServiceFactory : TerraformServiceFactory
    {
        public ResourceServiceFactory(IServiceProvider serviceProvider, DynamicDefinitionResolver dynamicResolver)
            : base(serviceProvider, dynamicResolver)
        {
        }

        public ResourceService Build(ResourceServiceDescriptor resourceDescriptor) =>
            new ResourceService(
                BuildSchema(resourceDescriptor),
                BuildService<INameProvider>(resourceDescriptor));
    }
}
