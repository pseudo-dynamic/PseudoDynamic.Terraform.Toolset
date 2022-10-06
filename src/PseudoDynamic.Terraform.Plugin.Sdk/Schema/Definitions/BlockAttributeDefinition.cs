using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.Definitions
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal class BlockAttributeDefinition : TerraformDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.BlockAttribute;

        /// <summary>
        /// The attribute name.
        /// </summary>
        public string AttributeName { get; }

        /// <summary>
        /// A specialized class that describes the attribute type.
        /// </summary>
        internal BlockSchemaPropertyInfo AttributeTypeInfo { get; }

        /// <summary>
        /// The intended Terraform type.
        /// </summary>
        public override TerraformType TerraformType => AttributeTypeInfo.TerraformType;

        /// <summary>
        /// The description of this attribute.
        /// </summary>
        public string Description {
            get => _description;
            init => _description = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// The value this attribute describes is required.
        /// </summary>
        public bool IsRequired { get; init; }

        /// <summary>
        /// The value this attribute describes is optional.
        /// </summary>
        public bool IsOptional { get; init; }

        /// <summary>
        /// The value this attribute describes will be computed.
        /// </summary>
        public bool IsComputed { get; init; }

        /// <summary>
        /// Marks this attribute as sensitive.
        /// </summary>
        public bool IsSensitive { get; init; }

        /// <summary>
        /// Kind of description.
        /// </summary>
        public DescriptionKind DescriptionKind { get; init; }

        /// <summary>
        /// Marks this attribute as deprecated.
        /// </summary>
        public bool Deprecated { get; init; }

        private string _description = string.Empty;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="attributeTypeInfo"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal BlockAttributeDefinition(string attributeName, BlockSchemaPropertyInfo attributeTypeInfo)
        {
            AttributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            AttributeTypeInfo = attributeTypeInfo ?? throw new ArgumentNullException(nameof(attributeTypeInfo));
        }
    }
}
