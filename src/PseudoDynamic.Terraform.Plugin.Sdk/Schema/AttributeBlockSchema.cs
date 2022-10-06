using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal record AttributeBlockSchema : AttributeSchema
    {
        public BlockSchema NestedBlock { get; init; } = BlockSchema.Empty;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="attributeTypeInfo"></param>
        /// <param name="terraformType">Accepts <see cref="TerraformType.Object"/> or <see cref="TerraformType.Block"/>.</param>
        /// <exception cref="InvalidOperationException">Incompatible Terraform type</exception>
        internal AttributeBlockSchema(
            string attributeName,
            BlockSchemaPropertyInfo attributeTypeInfo,
            TerraformType terraformType)
            : base(attributeName, attributeTypeInfo)
        {
            if (!terraformType.IsBlockType()) {
                throw new InvalidOperationException("Incompatible Terraform type");
            }
        }
    }
}
