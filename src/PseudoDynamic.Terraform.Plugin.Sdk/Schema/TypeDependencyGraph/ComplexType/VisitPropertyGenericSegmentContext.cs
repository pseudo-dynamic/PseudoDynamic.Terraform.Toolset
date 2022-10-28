using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal record VisitPropertyGenericSegmentContext : VisitContext, IVisitPropertySegmentContext
    {
        internal static VisitPropertyGenericSegmentContext New(IVisitPropertySegmentContext underlyingContext, Type visitType) =>
            new VisitPropertyGenericSegmentContext(underlyingContext, visitType);

        /// <inheritdoc/>
        public override VisitContextType ContextType { get; internal init; } = VisitContextType.PropertyGenericSegment;

        public PropertyInfo Property => _underlyingContext.Property;

        /// <summary>
        /// The declaring type that contains the generic type <see cref="VisitContext.VisitType"/> at index <see cref="GenericArgumentIndex"/>.
        /// </summary>
        public Type DeclaringType => NullabilityInfo.Type;

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public InheritableNullabilityInfo NullabilityInfo { get; }

        IVisitPropertySegmentContext _underlyingContext;

        private VisitPropertyGenericSegmentContext(IVisitPropertySegmentContext underlyingContext, Type visitType)
            : base(underlyingContext, visitType)
        {
            _underlyingContext = underlyingContext;
            NullabilityInfo = new InheritableNullabilityInfo(visitType);
        }

        internal VisitPropertyGenericSegmentContext(IVisitPropertySegmentContext underlyingContext, Type genericArgument, int genericArgumentIndex)
            : base(underlyingContext, genericArgument)
        {
            _underlyingContext = underlyingContext;
            NullabilityInfo = underlyingContext.NullabilityInfo.GetGenericTypeArgument(genericArgumentIndex);
        }

        public override T? GetContextualAttribute<T>()
            where T : class =>
            _underlyingContext.GetContextualAttribute<T>();
    }
}
