using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal class MapDefinition : RangeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Map;

        public override TerraformType TerraformType => TerraformType.Map;

        public StringDefinition Key { get; } = StringDefinition.Default;

        public TerraformDefinition Value { get; }

        public MapDefinition(TerraformDefinition value)
        {
            Value = value;
        }
    }
}
