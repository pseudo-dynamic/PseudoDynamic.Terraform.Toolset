using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ResourceDefinition
    {
        public BlockDefinition Schema { get; }

        public IResourceInfo Resource { get; }

        public string ResourceTypeName { get; init; }

        public ResourceDefinition(BlockDefinition schema, IResourceInfo resource)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));

            var resourceTypeName = Resource.TypeName;
            TerraformNameConventionException.EnsureResourceTypeNameConvention(resourceTypeName);
            ResourceTypeName = resourceTypeName;
        }
    }
}
