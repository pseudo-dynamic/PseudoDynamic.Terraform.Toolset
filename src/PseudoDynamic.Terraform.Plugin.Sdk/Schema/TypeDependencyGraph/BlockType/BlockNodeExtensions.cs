namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    internal static class BlockNodeExtensions
    {
        internal static BlockNode<TContext> AsContext<TContext>(this BlockNode node) =>
            new BlockNode<TContext>(node);
    }
}
