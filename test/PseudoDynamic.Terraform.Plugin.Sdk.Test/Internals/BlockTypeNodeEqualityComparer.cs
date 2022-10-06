using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors;
using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class BlockTypeNodeEqualityComparer<TBlockTypeNode> : EqualityComparer<BlockTypeNode>
        where TBlockTypeNode : BlockTypeNodeBase<TBlockTypeNode>
    {
        public new static readonly BlockTypeNodeEqualityComparer<TBlockTypeNode> Default = new BlockTypeNodeEqualityComparer<TBlockTypeNode>();

        public override bool Equals(BlockTypeNode? x, BlockTypeNode? y)
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
                && x.Context.VisitedType == y.Context.VisitedType
                && x.Nodes.SequenceEqual(y.Nodes, Default);
        }

        public override int GetHashCode([DisallowNull] BlockTypeNode obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.Context.ContextType);
            hashCode.Add(obj.Context.VisitedType);
            return hashCode.ToHashCode();
        }
    }
}
