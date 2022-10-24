namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class MonoRangeDefinition : RangeDefinition, INestedValueProvider
    {
        internal static MonoRangeDefinition List<SourceType>(ValueDefinition item) =>
            new MonoRangeDefinition(typeof(SourceType), TerraformTypeConstraint.List, item);

        internal static MonoRangeDefinition ListUncomputed(ValueDefinition item) =>
            new MonoRangeDefinition(UncomputedSourceType, TerraformTypeConstraint.List, item);

        internal static MonoRangeDefinition Set<SourceType>(ValueDefinition item) =>
            new MonoRangeDefinition(typeof(SourceType), TerraformTypeConstraint.Set, item);

        internal static MonoRangeDefinition SetUncomputed(ValueDefinition item) =>
            new MonoRangeDefinition(UncomputedSourceType, TerraformTypeConstraint.Set, item);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.MonoRange;

        public override TerraformTypeConstraint TypeConstraint { get; }

        public ValueDefinition Item { get; }
        ValueDefinition INestedValueProvider.NestedValue => Item;

        public MonoRangeDefinition(Type sourceType, TerraformTypeConstraint typeConstraint, ValueDefinition item)
            : base(sourceType)
        {
            TypeConstraint = typeConstraint;
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitMonoRange(this);

        public virtual bool Equals(MonoRangeDefinition? other) => base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
