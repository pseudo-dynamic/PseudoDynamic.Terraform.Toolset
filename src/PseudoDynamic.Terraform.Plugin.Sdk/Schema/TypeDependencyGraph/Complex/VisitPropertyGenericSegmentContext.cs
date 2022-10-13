using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex
{
    internal record VisitPropertyGenericSegmentContext : VisitContext, IVisitPropertySegmentContext
    {
        /// <inheritdoc/>
        public override VisitContextType ContextType { get; internal init; } = VisitContextType.PropertyGenericSegment;

        public PropertyInfo Property { get; }

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

        internal VisitPropertyGenericSegmentContext(IVisitPropertySegmentContext underlyingContext, Type genericArgument, int genericArgumentIndex)
            : base(underlyingContext, genericArgument)
        {
            SegmentDepth = underlyingContext.SegmentDepth + 1;
            Property = underlyingContext.Property;
            NullabilityInfo = underlyingContext.NullabilityInfo.GenericTypeArguments[genericArgumentIndex];
            GenericArgumentIndex = genericArgumentIndex;
        }
    }
}
