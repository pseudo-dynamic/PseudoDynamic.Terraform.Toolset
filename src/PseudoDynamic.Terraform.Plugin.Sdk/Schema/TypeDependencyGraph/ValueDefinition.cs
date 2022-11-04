using CSF.Collections;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class ValueDefinition : TerraformDefinition
    {
        private static readonly ListEqualityComparer<TypeWrapping> TypeWrappingListEqualityComparer = new ListEqualityComparer<TypeWrapping>();

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

        public IReadOnlyList<TypeWrapping> SourceTypeWrapping {
            get => _sourceTypeWrapping ??= Array.Empty<TypeWrapping>();
            init => _sourceTypeWrapping = value;
        }

        public abstract TerraformTypeConstraint TypeConstraint { get; }

        private Type? _outerType;
        private IReadOnlyList<TypeWrapping>? _sourceTypeWrapping;

        protected ValueDefinition(Type sourceType) : base(sourceType)
        {
        }

        public virtual bool Equals(ValueDefinition? other) =>
            other is not null
            && base.Equals(other)
            && OuterType == other.OuterType
            && SourceTypeWrapping.SequenceEqual(other.SourceTypeWrapping)
            && TypeConstraint == other.TypeConstraint;

        public override int GetHashCode() => HashCode.Combine(
            OuterType,
            TypeWrappingListEqualityComparer.GetHashCode(SourceTypeWrapping),
            TypeConstraint);
    }
}
