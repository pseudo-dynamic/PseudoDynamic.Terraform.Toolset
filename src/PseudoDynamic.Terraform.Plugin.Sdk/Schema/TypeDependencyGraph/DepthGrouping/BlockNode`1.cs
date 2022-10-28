using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping
{
    internal record class BlockNode<TContext> : BlockNode
    {
        public VisitContext OriginalContext { get; private set; }
        public new TContext Context { get; private set; }

        internal BlockNode(VisitContext context)
            : base(context) =>
            SetContext(context);

        public BlockNode(BlockNode node)
            : base(node) =>
            SetContext(node.Context);

        [MemberNotNull(nameof(Context), nameof(OriginalContext))]
        private void SetContext(VisitContext context2)
        {
            if (context2 is not TContext context) {
                throw new ArgumentException($"The node context of type {context2} is not compatible with {typeof(TContext).FullName}");
            }

            OriginalContext = context2;
            Context = context;
        }
    }
}
