using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal record VisitingPropertyGenericArgumentContext : VisitingContext, IVisitingPropertyArgumentContext
    {
        /// <inheritdoc/>
        public override VisitingContextType ContextType => VisitingContextType.PropertyGenericArgument;

        /// <summary>
        /// The declaring type that contains the generic type <see cref="VisitingContext.VisitingType"/> at index <see cref="GenericArgumentIndex"/>.
        /// </summary>
        public Type DeclaringType => GenericArgumentNullabilityInfo.Type;

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public NullabilityInfo GenericArgumentNullabilityInfo { get; }
        NullabilityInfo IVisitingPropertyArgumentContext.NullabilityInfo => GenericArgumentNullabilityInfo;

        /// <summary>
        /// The index of generic argument.
        /// </summary>
        public int GenericArgumentIndex { get; }

        internal VisitingPropertyGenericArgumentContext(IVisitingPropertyArgumentContext underlyingContext, Type genericArgument, int genericArgumentIndex)
            : base(underlyingContext, genericArgument)
        {
            GenericArgumentNullabilityInfo = underlyingContext.NullabilityInfo.GenericTypeArguments[genericArgumentIndex];
            GenericArgumentIndex = genericArgumentIndex;
        }
    }
}
