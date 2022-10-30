using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal interface ITerraformService<out TService>
        where TService : class
    {
        BlockDefinition Schema { get; }
        TService Service { get; }
        TypeAccessor ServiceTypeAccessor { get; }
    }
}
