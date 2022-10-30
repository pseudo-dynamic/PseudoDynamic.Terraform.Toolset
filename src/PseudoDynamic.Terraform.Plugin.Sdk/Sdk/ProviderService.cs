using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ProviderService : TerraformService<object>
    {
        public ProviderService(BlockDefinition schema, object instance)
            : base(schema, instance)
        {
        }
    }
}
