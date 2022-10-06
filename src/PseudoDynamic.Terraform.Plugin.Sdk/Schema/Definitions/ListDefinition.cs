using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal class ListDefinition : MonoRangeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.List;
        public override TerraformType TerraformType => TerraformType.List;

        public ListDefinition(TerraformDefinition item)
            : base(item)
        {
        }
    }
}
