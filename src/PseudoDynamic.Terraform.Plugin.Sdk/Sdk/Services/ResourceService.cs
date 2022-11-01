using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class ResourceService : TerraformService<IResource>
    {
        public ResourceService(BlockDefinition schema, IResource resource)
            : base(schema, resource) =>
            TerraformNameConventionException.EnsureResourceTypeNameConvention(resource.Name);
    }
}
