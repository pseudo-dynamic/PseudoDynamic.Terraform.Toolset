using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal class TupleDefinition : RangeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Tuple;

        public override TerraformType TerraformType => TerraformType.Tuple;
    }
}
