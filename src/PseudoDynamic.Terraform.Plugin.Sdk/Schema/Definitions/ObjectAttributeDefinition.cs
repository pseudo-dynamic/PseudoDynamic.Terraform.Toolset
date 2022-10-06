using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    internal class ObjectAttributeDefinition : TerraformDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.ObjectAttribute;
        public override TerraformType TerraformType => TerraformType.Object;

        /// <summary>
        /// The value this attribute describes is required.
        /// </summary>
        public bool IsRequired { get; init; }

        /// <summary>
        /// The value this attribute describes is optional.
        /// </summary>
        public bool IsOptional { get; init; }
    }
}
