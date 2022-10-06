using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal interface IRootVisitContext
    {
        NullabilityInfoContext NullabilityInfoContext { get; }
        IReadOnlySet<Type> VisitedComplexTypes { get; }
    }
}
