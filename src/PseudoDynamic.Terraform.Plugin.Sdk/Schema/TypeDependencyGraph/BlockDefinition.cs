namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class BlockDefinition : ComplexDefinition
    {
        internal const int DefaultSchemaVersion = 1;

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Block;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Block;

        public int SchemaVersion { get; init; } = DefaultSchemaVersion;

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitBlock(this);
    }
}
