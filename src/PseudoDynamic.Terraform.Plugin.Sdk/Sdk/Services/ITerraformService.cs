using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal interface ITerraformService<out TService>
    {
        BlockDefinition Schema { get; }
        TService Implementation { get; }
    }
}
