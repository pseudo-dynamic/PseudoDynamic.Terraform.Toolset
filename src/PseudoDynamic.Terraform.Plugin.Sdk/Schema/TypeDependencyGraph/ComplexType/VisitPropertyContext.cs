using System.Reflection;
using Namotion.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal record class VisitPropertyContext : VisitContext, IVisitPropertySegmentContext
    {
        /// <inheritdoc/>
        public override VisitContextType ContextType { get; internal init; }

        public PropertyInfo Property { get; }

        /// <summary>
        /// The nullability info of the walking property.
        /// </summary>
        public InheritableNullabilityInfo NullabilityInfo { get; }

        private ContextualPropertyInfo? _contextualProperty;

        internal VisitPropertyContext(IContext context, PropertyInfo property)
            : base(context, property.PropertyType)
        {
            ContextType = VisitContextType.Property;
            Property = property;
            NullabilityInfo = new OriginalNullabilityInfo(NullabilityInfoContext.Create(property));
        }

        internal VisitPropertyContext(IVisitPropertySegmentContext underlyingSegment, Type segmentType)
            : base(underlyingSegment, segmentType)
        {
            ContextType = VisitContextType.PropertySegment;
            Property = underlyingSegment.Property;
            NullabilityInfo = underlyingSegment.NullabilityInfo;
        }

        private ContextualPropertyInfo GetContextualProperty() =>
            _contextualProperty ??= Property.ToContextualProperty();

        public override T? GetContextualAttribute<T>()
            where T : class =>
            GetContextualProperty().GetContextAttribute<T>();

        private record OriginalNullabilityInfo : InheritableNullabilityInfo
        {
            public override NullabilityState ReadState => _nullabilityInfo.ReadState;

            private readonly NullabilityInfo _nullabilityInfo;

            internal OriginalNullabilityInfo(NullabilityInfo nullabilityInfo) : base(nullabilityInfo.Type) =>
                _nullabilityInfo = nullabilityInfo ?? throw new ArgumentNullException(nameof(nullabilityInfo));

            public override InheritableNullabilityInfo GetGenericTypeArgument(int index) =>
                new OriginalNullabilityInfo(_nullabilityInfo.GenericTypeArguments[index]);
        }
    }
}
