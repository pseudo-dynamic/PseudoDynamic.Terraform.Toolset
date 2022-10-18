using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping
{
    internal class BlockNodeContextEqualityComparer : EqualityComparer<BlockNode>
    {
        public new static readonly BlockNodeContextEqualityComparer Default = new BlockNodeContextEqualityComparer();

        public override bool Equals(BlockNode? x, BlockNode? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Context.ContextType == y.Context.ContextType
                && x.Context.VisitType == y.Context.VisitType
                && x.Nodes.SequenceEqual(y.Nodes, Default);
        }

        public override int GetHashCode([DisallowNull] BlockNode obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.Context.ContextType);
            hashCode.Add(obj.Context.VisitType);
            return hashCode.ToHashCode();
        }
    }
}
