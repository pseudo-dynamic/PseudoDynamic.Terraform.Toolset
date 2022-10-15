namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class BlockAttributeDefinition : BlockAttributeDefinitionBase
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.BlockAttribute;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BlockAttributeDefinition(string name, ValueDefinition value)
            : base(name, value)
        {
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitBlockAttribute(this);
    }
}
