using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ResourceService : TerraformService<INameProvider>
    {
        public ResourceService(BlockDefinition schema, INameProvider resource)
            : base(schema, resource) =>
            TerraformNameConventionException.EnsureResourceTypeNameConvention(resource.Name);
    }
}
