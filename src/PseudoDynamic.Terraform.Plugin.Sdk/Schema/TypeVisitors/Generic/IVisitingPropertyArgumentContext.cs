using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal interface IVisitingPropertyArgumentContext : IVisitingContext
    {
        NullabilityInfo NullabilityInfo { get; }
    }
}
