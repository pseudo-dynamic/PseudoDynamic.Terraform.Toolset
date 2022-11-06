using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal class ProviderServiceFactory : TerraformServiceFactory
    {
        public ProviderServiceFactory(IServiceProvider serviceProvider, SchemaBuilder schemaBuilder)
            : base(serviceProvider, schemaBuilder)
        {
        }

        public ProviderService Build(ProviderServiceDescriptor descriptor) =>
            new(BuildSchema(descriptor.SchemaType),
                CreateImplementation<IProvider>(descriptor));

        public ProviderService BuildUnimplemented() =>
            ProviderService.Unimplemented(BlockDefinition.Uncomputed);
    }
}
