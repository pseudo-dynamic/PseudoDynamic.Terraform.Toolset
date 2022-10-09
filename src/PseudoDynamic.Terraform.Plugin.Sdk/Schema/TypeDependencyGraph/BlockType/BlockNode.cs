using System.Collections;
using System.Diagnostics;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    internal record class BlockNode : IEnumerable<BlockNode>
    {
        internal VisitContext Context { get; }

        public IReadOnlyList<BlockNode> Nodes => _nodes;

        private readonly List<BlockNode> _nodes;

        internal BlockNode(VisitContext context)
        {
            Context = context;
            _nodes = new List<BlockNode>();
        }

        internal BlockNode(VisitContext context, IEnumerable<BlockNode> nodes)
        {
            Context = context;
            _nodes = new List<BlockNode>(nodes);
        }

        internal void Add(BlockNode node) =>
            _nodes.Add(node);

        /// <summary>
        /// Enumerates <see cref="Nodes"/>.
        /// </summary>
        public IEnumerator<BlockNode> GetEnumerator() => Nodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private string GetDebuggerDisplay() =>
            $"[{Context.ContextType}, {Context.VisitedType.Name}, Nodes = {Nodes.Count}]";
    }
}
