using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping
{
    internal sealed class TerraformVisitContextType : VisitContextType
    {
        public static readonly VisitContextType TerraformValue = New().Inherits(PropertyGenericSegment);
    }
}
