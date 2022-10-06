using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal class SetDefinition : MonoRangeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Set;

        public override TerraformType TerraformType => TerraformType.Set;

        public SetDefinition(TerraformDefinition item)
            : base(item)
        {
        }
    }
}
