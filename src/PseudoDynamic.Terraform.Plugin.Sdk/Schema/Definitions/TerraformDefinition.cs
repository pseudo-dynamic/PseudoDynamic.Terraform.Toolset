using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal abstract class TerraformDefinition
    {
        public abstract TerraformDefinitionType DefinitionType { get; }

        public abstract TerraformType TerraformType { get; }
    }
}
