using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    internal record class BlockNode<TContext> : BlockNode
    {
        public new TContext Context { get; private set; }

        internal BlockNode(VisitContext context)
            : base(context) =>
            SetContext(context);

        public BlockNode(BlockNode node)
            : base(node) =>
            SetContext(node.Context);

        [MemberNotNull(nameof(Context))]
        private void SetContext(VisitContext context)
        {
            if (context is not TContext typedContext) {
                throw new ArgumentException($"The node context of type {context} is not compatible with {typeof(TContext).FullName}");
            }

            Context = typedContext;
        }
    }
}
