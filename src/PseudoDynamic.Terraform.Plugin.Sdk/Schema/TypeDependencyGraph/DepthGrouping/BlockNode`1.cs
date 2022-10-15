namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping
{
    internal record class BlockNode<TContext> : BlockNode
    {
        public new TContext Context { get; }

        public BlockNode(BlockNode node)
            : base(node)
        {
            if (node.Context is not TContext context) {
                throw new ArgumentException($"The node context of type {node.Context} is not compatible with {typeof(TContext).FullName}");
            }

            Context = context;
        }
    }
}
