using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

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

        private static void EnsureNodesNotHavingNestedTerraformValue(this BlockNode node)
        {
            if (node.Any(childNode => childNode.Context.ContextType == BlockVisitContextType.TerraformValue)) {
                var errorMessage = node.Context.ContextType == BlockVisitContextType.TerraformValue
                    ? $"In the dependency graph of {node.Context.VisitType.FullName} is another nested terraform value not allowed"
                    : $"In the dependency graph of {node.Context.VisitType.FullName} is a nested terraform value not allowed";

                throw new NestedTerraformValueException(errorMessage);
            }
        }

        private static bool TryUnwrapTerraformValue(this BlockNode<IVisitPropertySegmentContext> node, out BlockNode<IVisitPropertySegmentContext> unwrappedNode)
        {
            if (node.SingleOrDefault(childNode => childNode.Context.ContextType == BlockVisitContextType.TerraformValue) is BlockNode childNode) {
                unwrappedNode = childNode.AsContext<IVisitPropertySegmentContext>();
                unwrappedNode.EnsureNodesNotHavingNestedTerraformValue();
                return true;
            }

            unwrappedNode = node;
            return false;
        }

        private static bool TryUnwrapNullable(this BlockNode<IVisitPropertySegmentContext> node, out BlockNode<IVisitPropertySegmentContext> unwrappedNode)
        {
            if (node.SingleOrDefault(childNode => childNode.Context.ContextType == BlockVisitContextType.Nullable) is BlockNode childNode) {
                unwrappedNode = childNode.AsContext<IVisitPropertySegmentContext>();
                return true;
            }

            unwrappedNode = node;
            return false;
        }

        public static IReadOnlyList<TypeWrapping> TryUnwrap(this BlockNode<IVisitPropertySegmentContext> node, out BlockNode<IVisitPropertySegmentContext> unwrappedNode)
        {
            var typeWrappings = new List<TypeWrapping>();

            if (node.TryUnwrapTerraformValue(out unwrappedNode)) {
                typeWrappings.Add(TypeWrapping.TerraformValue);

                if (unwrappedNode.TryUnwrapNullable(out unwrappedNode)) {
                    typeWrappings.Add(TypeWrapping.Nullable);
                }
            } else if (node.TryUnwrapNullable(out unwrappedNode)) {
                typeWrappings.Add(TypeWrapping.Nullable);
            }

            return typeWrappings;
        }
    }
}
