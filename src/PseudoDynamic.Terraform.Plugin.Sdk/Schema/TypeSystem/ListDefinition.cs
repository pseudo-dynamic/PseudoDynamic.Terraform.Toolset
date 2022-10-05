using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeSystem
{
    internal class ListDefinition : MonoRangeDefinition
    {
        public override TerraformType TerraformType => TerraformType.List;
    }
}
