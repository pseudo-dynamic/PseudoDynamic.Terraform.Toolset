namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class ValueDefinition : TerraformDefinition
    {
        public Type WrappedSourceType {
            get => _wrappedSourceType ?? SourceType;
            init => _wrappedSourceType = value;
        }

        public bool IsWrappedByTerraformValue { get; init; }

        public abstract TerraformTypeConstraint TypeConstraint { get; }

        private Type? _wrappedSourceType;

        protected ValueDefinition(Type sourceType) : base(sourceType)
        {
        }

        public virtual bool Equals(ValueDefinition? other) =>
            other is not null
            && base.Equals(other)
            && WrappedSourceType == other.WrappedSourceType
            && IsWrappedByTerraformValue == other.IsWrappedByTerraformValue
            && TypeConstraint == other.TypeConstraint;

        public override int GetHashCode() => HashCode.Combine(
            WrappedSourceType,
            IsWrappedByTerraformValue,
            TypeConstraint);
    }
}
