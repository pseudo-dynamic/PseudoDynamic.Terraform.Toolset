using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitPropertySegmentContext : IVisitContext
    {
        PropertyInfo Property { get; }

        NullabilityInfo NullabilityInfo { get; }

        /// <summary>
        /// The property segment depth starting with zero.
        /// </summary>
        int SegmentDepth { get; }
    }
}
