using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeSystem
{
    internal class TupleDefinition : RangeDefinition
    {
        public override TerraformType TerraformType => TerraformType.Tuple;
    }
}
