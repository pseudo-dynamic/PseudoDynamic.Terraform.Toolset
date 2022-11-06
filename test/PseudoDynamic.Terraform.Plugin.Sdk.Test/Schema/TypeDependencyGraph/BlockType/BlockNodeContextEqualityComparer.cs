using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    internal class BlockNodeContextEqualityComparer : EqualityComparer<BlockNode>
    {
        public static new readonly BlockNodeContextEqualityComparer Default = new();

        public override bool Equals(BlockNode? x, BlockNode? y)
        {
            if (x is null && y is null) {
                return true;
            }

            if (x is null || y is null) {
                return false;
            }

            return x.Context.ContextType == y.Context.ContextType
                && x.Context.VisitType == y.Context.VisitType
                && x.Nodes.SequenceEqual(y.Nodes, Default);
        }

        public override int GetHashCode([DisallowNull] BlockNode obj) => HashCode.Combine(
            obj.Context.ContextType,
            obj.Context.VisitType);
    }
}
