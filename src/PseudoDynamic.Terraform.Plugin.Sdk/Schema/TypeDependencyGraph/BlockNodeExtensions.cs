using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal static class BlockNodeExtensions
    {
        public static bool TryUnwrapProperty(this BlockNode node, out BlockNode<IVisitPropertySegmentContext> unwrappedNode)
        {
            if (node.Context.ContextType == VisitContextType.Property && node.Nodes.Count != 0) {
                unwrappedNode = node.Single().AsContext<IVisitPropertySegmentContext>();
                return true;
            }

            unwrappedNode = node.AsContext<IVisitPropertySegmentContext>();
            return false;
        }

        public static bool TryUnwrapTerraformValue(this BlockNode node, out BlockNode<IVisitPropertySegmentContext> unwrappedNode)
        {
            if (node.SingleOrDefault(childNode => childNode.Context.ContextType == TerraformVisitContextType.TerraformValue) is BlockNode childNode) {
                unwrappedNode = childNode.AsContext<IVisitPropertySegmentContext>();
                unwrappedNode.EnsureNodesNotHavingNestedTerraformValue();
                return true;
            }

            unwrappedNode = node.AsContext<IVisitPropertySegmentContext>();
            return false;
        }

        public static void EnsureNodesNotHavingNestedTerraformValue(this BlockNode node)
        {
            if (node.Any(x => x.Context.ContextType == TerraformVisitContextType.TerraformValue)) {
                var errorMessage = node.Context.ContextType == TerraformVisitContextType.TerraformValue
                    ? $"In the dependency graph of {node.Context.VisitType.FullName}, another nested terraform value is not allowed"
                    : $"In the dependency graph of {node.Context.VisitType.FullName}, a nested terraform value is not allowed";

                throw new NestedTerraformValueException(errorMessage);
            }
        }
    }
}
