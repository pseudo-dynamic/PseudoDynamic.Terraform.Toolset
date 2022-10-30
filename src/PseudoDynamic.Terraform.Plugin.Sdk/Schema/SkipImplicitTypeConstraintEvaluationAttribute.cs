using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// If a type gets visited that is annotated by this attribute, then the type constraint of annotated type won't be implicitly
    /// evaluated. This is reasonable when the type constraint evaluation is not required for example for <see cref="VisitContext"/>
    /// representing <see cref="ITerraformValue{T}"/> inside a type dependency graph, that would otherwise result into "bad guessing".
    /// This mechanic is used for example in <see cref="BlockNodeBuilder"/> when rewriting property segments that depend on these
    /// implicit evaluated type constraints.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    internal class SkipImplicitTypeConstraintEvaluationAttribute : Attribute
    {
    }
}
