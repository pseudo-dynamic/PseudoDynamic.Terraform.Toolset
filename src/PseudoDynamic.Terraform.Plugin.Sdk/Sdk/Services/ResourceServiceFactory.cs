using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal class ResourceServiceFactory : TerraformServiceFactory
    {
        public ResourceServiceFactory(IServiceProvider serviceProvider, SchemaBuilder schemaBuilder)
            : base(serviceProvider, schemaBuilder)
        {
        }

        public ResourceService Build(ResourceServiceDescriptor descriptor) =>
            new ResourceService(
                BuildSchema(descriptor.SchemaType),
                CreateImplementation<IResource>(descriptor));
    }
}
