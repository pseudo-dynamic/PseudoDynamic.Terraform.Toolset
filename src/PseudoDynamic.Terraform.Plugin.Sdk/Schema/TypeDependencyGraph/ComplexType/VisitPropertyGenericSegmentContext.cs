using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal record VisitPropertyGenericSegmentContext : VisitContext, IVisitPropertySegmentContext
    {
        /// <inheritdoc/>
        public override VisitContextType ContextType { get; internal init; } = VisitContextType.PropertyGenericSegment;

        public PropertyInfo Property => _underlyingContext.Property;

        /// <summary>
        /// The declaring type that contains the generic type <see cref="VisitContext.VisitedType"/> at index <see cref="GenericArgumentIndex"/>.
        /// </summary>
        public Type DeclaringType => NullabilityInfo.Type;

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public NullabilityInfo NullabilityInfo { get; }

        /// <summary>
        /// The index of generic argument.
        /// </summary>
        public int GenericArgumentIndex { get; }

        public int SegmentDepth { get; }

        IVisitPropertySegmentContext _underlyingContext;

        internal VisitPropertyGenericSegmentContext(IVisitPropertySegmentContext underlyingContext, Type genericArgument, int genericArgumentIndex)
            : base(underlyingContext, genericArgument)
        {
            _underlyingContext = underlyingContext;
            SegmentDepth = underlyingContext.SegmentDepth + 1;
            NullabilityInfo = underlyingContext.NullabilityInfo.GenericTypeArguments[genericArgumentIndex];
            GenericArgumentIndex = genericArgumentIndex;
        }

        public override T? GetContextualAttribute<T>()
            where T : class =>
            _underlyingContext.GetContextualAttribute<T>();
    }
}
