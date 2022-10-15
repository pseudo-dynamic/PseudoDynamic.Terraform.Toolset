namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping
{
    internal static class BlockNodeExtensions
    {
        internal static BlockNode<TContext> AsContext<TContext>(this BlockNode node) =>
            new BlockNode<TContext>(node);
    }
}
