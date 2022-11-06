using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.NullabilityAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal record VisitPropertyGenericSegmentContext : VisitContext, IVisitPropertySegmentContext
    {
        /// <summary>
        /// Creates a custom property generic segment, that inherits a pre-existing underlying context.
        /// </summary>
        /// <param name="underlyingContext"></param>
        /// <param name="visitType"></param>
        /// <param name="contextType"></param>
        /// <remarks>
        /// Breaks the nullability chain of this and next property generic segments. They are treated as they would be annotated by <see cref="MaybeNullAttribute"/>.
        /// </remarks>
        internal static VisitPropertyGenericSegmentContext Custom(IVisitPropertySegmentContext underlyingContext, Type visitType, VisitContextType contextType) =>
            new(underlyingContext, visitType) { ContextType = contextType };

        /// <summary>
        /// Creates a custom property generic segment, that inherits a pre-existing underlying context.
        /// </summary>
        /// <param name="underlyingContext"></param>
        /// <param name="visitType"></param>
        /// <remarks>
        /// Breaks the nullability chain of this and next property generic segments. They are treated as they would be annotated by <see cref="MaybeNullAttribute"/>.
        /// </remarks>
        internal static VisitPropertyGenericSegmentContext Custom(IVisitPropertySegmentContext underlyingContext, Type visitType) =>
            new(underlyingContext, visitType);

        /// <inheritdoc/>
        public override VisitContextType ContextType { get; internal init; } = VisitContextType.PropertyGenericSegment;

        public PropertyInfo Property => _underlyingContext.Property;

        /// <summary>
        /// The declaring type that contains the generic type <see cref="VisitContext.VisitType"/>.
        /// </summary>
        public Type DeclaringType => NullabilityInfo.Type;

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public AbstractNullablityInfo NullabilityInfo { get; }

        private readonly IVisitPropertySegmentContext _underlyingContext;

        private VisitPropertyGenericSegmentContext(IVisitPropertySegmentContext underlyingContext, Type visitType)
            : base(underlyingContext, visitType)
        {
            _underlyingContext = underlyingContext;
            NullabilityInfo = new NullableInfo(visitType);
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
