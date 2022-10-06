using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal sealed class StringDefinition : TerraformDefinition
    {
        public static readonly StringDefinition Default = new StringDefinition();

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.String;

        public override TerraformType TerraformType => TerraformType.String;

        private StringDefinition()
        {
        }
    }
}
