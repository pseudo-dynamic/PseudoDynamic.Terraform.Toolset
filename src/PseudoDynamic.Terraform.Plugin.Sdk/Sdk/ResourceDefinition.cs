using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ResourceDefinition
    {
        public BlockDefinition Schema { get; }

        public IResourceInfo Resource { get; }

        public TypeAccessor ResourceAccessor { get; }

        public ResourceDefinition(BlockDefinition schema, IResourceInfo resource)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            ResourceAccessor = new TypeAccessor(Resource.GetType());
            TerraformNameConventionException.EnsureResourceTypeNameConvention(Resource.TypeName);
        }
    }
}
