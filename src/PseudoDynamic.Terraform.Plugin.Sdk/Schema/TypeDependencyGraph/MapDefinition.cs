namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class MapDefinition : RangeDefinition, INestedValueProvider
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Map;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Map;

        // CONSIDER: allow key type other than string
        public ValueDefinition Key { get; } = PrimitiveDefinition.String;

        public ValueDefinition Value { get; }
        ValueDefinition INestedValueProvider.NestedValue => Value;

        public MapDefinition(Type sourceType, ValueDefinition value)
            : base(sourceType) =>
            Value = value ?? throw new ArgumentNullException(nameof(value));

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitMap(this);

        public virtual bool Equals(MapDefinition? other) => base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
