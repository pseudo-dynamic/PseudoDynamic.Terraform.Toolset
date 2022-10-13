using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Block
{
    internal sealed class TerraformVisitContextType : VisitContextType
    {
        public static readonly VisitContextType TerraformValue = New().Inherits(PropertyGenericSegment);
    }
}
