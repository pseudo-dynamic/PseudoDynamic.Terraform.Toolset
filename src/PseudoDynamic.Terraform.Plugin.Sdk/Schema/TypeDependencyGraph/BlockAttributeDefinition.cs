namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class BlockAttributeDefinition : BlockAttributeDefinitionBase
    {
        public static BlockAttributeDefinition Uncomputed(string name, ValueDefinition value) =>
            new(UncomputedSourceType, name, value);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.BlockAttribute;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BlockAttributeDefinition(Type sourceType, string name, ValueDefinition value)
            : base(sourceType, name, value)
        {
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitBlockAttribute(this);
    }
}
