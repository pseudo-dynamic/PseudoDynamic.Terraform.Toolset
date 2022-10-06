using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal class BlockTypeNode : BlockTypeNodeBase<BlockTypeNode>
    {
        internal BlockTypeNode(VisitContext context)
            : base(context)
        {
        }

        internal BlockTypeNode(VisitContext context, IEnumerable<BlockTypeNode> nodes)
            : base(context, nodes)
        {
        }
    }
}
