namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class MapDefinition : RangeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Map;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Map;

        public ValueDefinition Key { get; } = PrimitiveDefinition.String;

        public ValueDefinition Value { get; }

        public MapDefinition(ValueDefinition value) =>
            Value = value ?? throw new ArgumentNullException(nameof(value));

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitMap(this);

        public virtual bool Equals(MapDefinition? other) => base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
