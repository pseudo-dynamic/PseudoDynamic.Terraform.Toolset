using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal record VisitPropertyContext : VisitContext, IVisitPropertySegmentContext
    {
        /// <summary>
        /// The current walking property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public NullabilityInfo PropertyNullabilityInfo { get; }
        NullabilityInfo IVisitPropertySegmentContext.NullabilityInfo => PropertyNullabilityInfo;

        /// <inheritdoc/>
        public override VisitContextType ContextType => VisitContextType.Property;

        internal VisitPropertyContext(IRootVisitContext context, PropertyInfo property)
            : base(context, property.PropertyType)
        {
            Property = property;
            PropertyNullabilityInfo = NullabilityInfoContext.Create(property);
        }
    }
}
