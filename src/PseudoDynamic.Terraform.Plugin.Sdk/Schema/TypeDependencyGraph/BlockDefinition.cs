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

        public IReadOnlyList<NestedBlockAttributeDefinition> Blocks { get; init; } = Array.Empty<NestedBlockAttributeDefinition>();

        /// <summary>
        /// The description of this attribute.
        /// </summary>
        public string Description {
            get => _description;
            init => _description = value ?? string.Empty;
        }

        /// <summary>
        /// Kind of description.
        /// </summary>
        public DescriptionKind DescriptionKind { get; init; }

        /// <summary>
        /// Marks this block as deprecated.
        /// </summary>
        public bool IsDeprecated { get; init; }

        private string _description = string.Empty;

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitBlock(this);

        public virtual bool Equals(BlockDefinition? other) =>
            other is not null
            && SchemaVersion == other.SchemaVersion
            && Description == other.Description
            && DescriptionKind == other.DescriptionKind
            && IsDeprecated == other.IsDeprecated;

        public override int GetHashCode() => HashCode.Combine(
            SchemaVersion,
            Description,
            DescriptionKind,
            IsDeprecated);
    }
}
