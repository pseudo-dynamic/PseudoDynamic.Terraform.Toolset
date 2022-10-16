﻿using System.Reflection;
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
        public NullabilityInfo NullabilityInfo { get; }

        public int SegmentDepth { get; }

        private ContextualPropertyInfo? _contextualProperty;

        internal VisitPropertyContext(IContext context, PropertyInfo property)
            : base(context, property.PropertyType)
        {
            SegmentDepth = 0;
            ContextType = VisitContextType.Property;
            Property = property;
            NullabilityInfo = NullabilityInfoContext.Create(property);
        }

        internal VisitPropertyContext(IVisitPropertySegmentContext underlyingSegment, Type segmentType)
            : base(underlyingSegment, segmentType)
        {
            SegmentDepth = underlyingSegment.SegmentDepth;
            ContextType = VisitContextType.PropertySegment;
            Property = underlyingSegment.Property;
            NullabilityInfo = underlyingSegment.NullabilityInfo;
        }

        private ContextualPropertyInfo GetContextualProperty() =>
            _contextualProperty ??= Property.ToContextualProperty();

        public override T? GetContextualAttribute<T>()
            where T : class =>
            GetContextualProperty().GetContextAttribute<T>();
    }
}
