namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class ValueDefinition : TerraformDefinition
    {
        /// <summary>
        /// If differing from <see cref="TerraformDefinition.SourceType"/>
        /// then <see cref="TerraformDefinition.SourceType"/> was computed
        /// from <see cref="DeclaringType"/>. It could then be the case,
        /// that <see cref="TerraformDefinition.SourceType"/> was used as
        /// type parameter of <see cref="DeclaringType"/>.
        /// </summary>
        public Type DeclaringType {
            get => _delcaringType ?? SourceType;
            init => _delcaringType = value;
        }

        /// <summary>
        /// If true, then <see cref="TerraformDefinition.SourceType"/> is
        /// wrapped by <see cref="ITerraformValue{T}"/> or of one of its
        /// derivative.
        /// </summary>
        public bool IsWrappedByTerraformValue { get; init; }

        public abstract TerraformTypeConstraint TypeConstraint { get; }

        private Type? _delcaringType;

        protected ValueDefinition(Type sourceType) : base(sourceType)
        {
        }

        public virtual bool Equals(ValueDefinition? other) =>
            other is not null
            && base.Equals(other)
            && DeclaringType == other.DeclaringType
            && IsWrappedByTerraformValue == other.IsWrappedByTerraformValue
            && TypeConstraint == other.TypeConstraint;

        public override int GetHashCode() => HashCode.Combine(
            DeclaringType,
            IsWrappedByTerraformValue,
            TypeConstraint);
    }
}
