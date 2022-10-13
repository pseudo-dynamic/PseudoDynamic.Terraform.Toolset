using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex
{
    internal interface IContext
    {
        NullabilityInfoContext NullabilityInfoContext { get; }
        IReadOnlySet<Type> VisitedComplexTypes { get; }
    }
}
