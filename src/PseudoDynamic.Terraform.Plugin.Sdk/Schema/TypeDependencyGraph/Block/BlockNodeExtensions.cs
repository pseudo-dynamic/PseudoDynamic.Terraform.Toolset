namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Block
{
    internal static class BlockNodeExtensions
    {
        internal static BlockNode<TContext> AsContext<TContext>(this BlockNode node) =>
            new BlockNode<TContext>(node);
    }
}
