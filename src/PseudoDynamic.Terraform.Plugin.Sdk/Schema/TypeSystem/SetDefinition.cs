using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeSystem
{
    internal class SetDefinition : MonoRangeDefinition
    {
        public override TerraformType TerraformType => TerraformType.Set;
    }
}
