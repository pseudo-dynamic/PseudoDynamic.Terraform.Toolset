using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    internal sealed class BlockVisitContextType : VisitContextType
    {
        public static readonly VisitContextType TerraformValue = New().Inherits(PropertyGenericSegment);
        public static readonly VisitContextType Nullable = New().Inherits(PropertyGenericSegment);
    }
}
