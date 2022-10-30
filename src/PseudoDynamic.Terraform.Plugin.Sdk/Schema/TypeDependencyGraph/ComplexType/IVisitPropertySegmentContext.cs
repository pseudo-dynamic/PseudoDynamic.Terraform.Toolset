using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.NullabilityAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitPropertySegmentContext : IVisitContext
    {
        PropertyInfo Property { get; }

        AbstractNullablityInfo NullabilityInfo { get; }
    }
}
