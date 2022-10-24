namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class ValueDefinition : TerraformDefinition
    {
        /// <summary>
        /// If differing from <see cref="TerraformDefinition.SourceType"/>
        /// then <see cref="TerraformDefinition.SourceType"/> was computed
        /// from <see cref="OuterType"/>. It could then be the case,
        /// that <see cref="TerraformDefinition.SourceType"/> was used as
        /// type parameter of <see cref="OuterType"/>.
        /// </summary>
        public Type OuterType {
            get => _outerType ?? SourceType;
            init => _outerType = value;
        }

        /// <summary>
        /// If true, then <see cref="TerraformDefinition.SourceType"/> is
        /// wrapped by <see cref="ITerraformValue{T}"/> or of one of its
        /// derivative.
        /// </summary>
        public bool IsWrappedByTerraformValue { get; init; }

        public abstract TerraformTypeConstraint TypeConstraint { get; }

        private Type? _outerType;

        protected ValueDefinition(Type sourceType) : base(sourceType)
        {
        }

        public virtual bool Equals(ValueDefinition? other) =>
            other is not null
            && base.Equals(other)
            && OuterType == other.OuterType
            && IsWrappedByTerraformValue == other.IsWrappedByTerraformValue
            && TypeConstraint == other.TypeConstraint;

        public override int GetHashCode() => HashCode.Combine(
            OuterType,
            IsWrappedByTerraformValue,
            TypeConstraint);
    }
}
