using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal interface IVisitPropertySegmentContext : IVisitContext
    {
        NullabilityInfo NullabilityInfo { get; }
    }
}
