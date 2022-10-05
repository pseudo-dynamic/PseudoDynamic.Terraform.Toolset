using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeSystem
{
    internal abstract class TypeDefinition
    {
        public abstract TerraformType TerraformType { get; }
    }
}
