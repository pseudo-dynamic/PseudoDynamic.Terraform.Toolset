using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal class BlockTypeNodeReducer
    {
        //protected virtual TerraformBlockTypeNode ReduceTerraformValue(BlockTypeNode node, Type terraformValueInterfaceGenericArgument) =>


        public TerraformBlockTypeNode Reduce(BlockTypeNode node)
        {
            var context = node.Context;
            var nodes = node.Nodes;

            if (context.ContextType == VisitContextType.Property || context.ContextType == VisitContextType.PropertyGenericArgument
                && TypeExtensions.IsImplementingGenericTypeDefinition(context.VisitedType, typeof(ITerraformValue<>), out _, out var genericTypeArguments)) {
                //return ReduceTerraformValue(node, genericTypeArguments.Single());
            }

            return new TerraformBlockTypeNode(context, nodes.Select(Reduce));
        }
    }
}
