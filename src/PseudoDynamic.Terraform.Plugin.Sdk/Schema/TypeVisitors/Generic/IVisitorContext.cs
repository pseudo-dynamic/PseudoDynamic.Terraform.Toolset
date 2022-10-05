using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal interface IVisitorContext
    {
        NullabilityInfoContext NullabilityInfoContext { get; }
        IReadOnlySet<Type> VisitedComplexTypes { get; }
    }
}
