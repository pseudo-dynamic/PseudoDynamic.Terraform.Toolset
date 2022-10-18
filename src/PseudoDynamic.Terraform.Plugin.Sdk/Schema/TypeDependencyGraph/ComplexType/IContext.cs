using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IContext
    {
        NullabilityInfoContext NullabilityInfoContext { get; }
        IReadOnlySet<Type> RememberedComplexVisitTypes { get; }
    }
}
