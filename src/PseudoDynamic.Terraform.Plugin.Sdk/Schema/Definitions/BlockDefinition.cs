using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal class BlockDefinition : TerraformDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Block;

        public override TerraformType TerraformType => TerraformType.Block;
    }
}
