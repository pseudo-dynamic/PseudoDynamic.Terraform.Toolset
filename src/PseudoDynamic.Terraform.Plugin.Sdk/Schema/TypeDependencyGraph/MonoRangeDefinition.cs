namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class MonoRangeDefinition : RangeDefinition, INestedValueAccessor
    {
        internal static MonoRangeDefinition List(ValueDefinition item) =>
            new MonoRangeDefinition(TerraformTypeConstraint.List, item);

        internal static MonoRangeDefinition Set(ValueDefinition item) =>
            new MonoRangeDefinition(TerraformTypeConstraint.Set, item);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.MonoRange;

        public override TerraformTypeConstraint TypeConstraint { get; }

        public ValueDefinition Item { get; }
        ValueDefinition INestedValueAccessor.NestedValue => Item;

        public MonoRangeDefinition(TerraformTypeConstraint typeConstraint, ValueDefinition item)
        {
            TypeConstraint = typeConstraint;
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitMonoRange(this);

        public virtual bool Equals(MonoRangeDefinition? other) =>
            base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
