using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal record VisitPropertyGenericArgumentContext : VisitContext, IVisitPropertySegmentContext
    {
        /// <inheritdoc/>
        public override VisitContextType ContextType { get; internal init; } = VisitContextType.PropertyGenericArgument;

        /// <summary>
        /// The declaring type that contains the generic type <see cref="VisitContext.VisitedType"/> at index <see cref="GenericArgumentIndex"/>.
        /// </summary>
        public Type DeclaringType => GenericArgumentNullabilityInfo.Type;

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public NullabilityInfo GenericArgumentNullabilityInfo { get; }
        NullabilityInfo IVisitPropertySegmentContext.NullabilityInfo => GenericArgumentNullabilityInfo;

        /// <summary>
        /// The index of generic argument.
        /// </summary>
        public int GenericArgumentIndex { get; }

        internal VisitPropertyGenericArgumentContext(IVisitPropertySegmentContext underlyingContext, Type genericArgument, int genericArgumentIndex)
            : base(underlyingContext, genericArgument)
        {
            GenericArgumentNullabilityInfo = underlyingContext.NullabilityInfo.GenericTypeArguments[genericArgumentIndex];
            GenericArgumentIndex = genericArgumentIndex;
        }
    }
}
