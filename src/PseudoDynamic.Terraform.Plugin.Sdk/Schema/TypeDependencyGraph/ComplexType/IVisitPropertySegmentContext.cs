using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitPropertySegmentContext : IVisitContext
    {
        PropertyInfo Property { get; }

        CustomNullabilityInfo NullabilityInfo { get; }
    }
}
