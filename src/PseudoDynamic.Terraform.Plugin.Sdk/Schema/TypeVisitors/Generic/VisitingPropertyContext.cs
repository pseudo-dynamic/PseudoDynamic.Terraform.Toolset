using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal record VisitingPropertyContext : VisitingContext, IVisitingPropertyArgumentContext
    {
        /// <summary>
        /// The current walking property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public NullabilityInfo PropertyNullabilityInfo { get; }
        NullabilityInfo IVisitingPropertyArgumentContext.NullabilityInfo => PropertyNullabilityInfo;

        /// <inheritdoc/>
        public override VisitingContextType ContextType => VisitingContextType.Property;

        internal VisitingPropertyContext(IVisitorContext context, PropertyInfo property)
            : base(context, property.PropertyType)
        {
            Property = property;
            PropertyNullabilityInfo = NullabilityInfoContext.Create(property);
        }
    }
}
