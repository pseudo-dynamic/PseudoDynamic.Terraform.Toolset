namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class ObjectDefinition : ComplexDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Object;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Object;

        public IReadOnlyList<ObjectAttributeDefinition> Attributes { get; init; } = Array.Empty<ObjectAttributeDefinition>();

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitObject(this);

        public virtual bool Equals(ObjectDefinition? other) => base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
