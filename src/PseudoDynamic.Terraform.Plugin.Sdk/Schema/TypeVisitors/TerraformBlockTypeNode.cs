using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal class TerraformBlockTypeNode : BlockTypeNodeBase<TerraformBlockTypeNode>
    {
        public Type? TerraformValueTypeDefinition { get; init; }

        public bool IsTerraformValue => TerraformValueTypeDefinition is not null;

        //internal TerraformBlockTypeNode(BlockTypeNode node)
        //    : base(node.Context, node.Nodes.ToTerraformBlockTypeNodes())
        //{
        //}

        internal TerraformBlockTypeNode(VisitContext context)
            : base(context)
        {
        }

        internal TerraformBlockTypeNode(VisitContext context, IEnumerable<TerraformBlockTypeNode> nodes)
            : base(context, nodes)
        {
        }
    }
}
