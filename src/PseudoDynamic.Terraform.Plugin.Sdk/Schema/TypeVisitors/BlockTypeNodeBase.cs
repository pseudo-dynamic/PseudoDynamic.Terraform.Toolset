using System.Collections;
using System.Diagnostics;
using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    internal abstract class BlockTypeNodeBase<TBlockTypeNode> : IEnumerable<TBlockTypeNode>
        where TBlockTypeNode : BlockTypeNodeBase<TBlockTypeNode>
    {
        internal VisitContext Context { get; }

        public IReadOnlyList<TBlockTypeNode> Nodes => _nodes;

        private readonly List<TBlockTypeNode> _nodes;

        internal BlockTypeNodeBase(VisitContext context)
        {
            Context = context;
            _nodes = new List<TBlockTypeNode>();
        }

        internal BlockTypeNodeBase(VisitContext context, IEnumerable<TBlockTypeNode> nodes)
        {
            Context = context;
            _nodes = new List<TBlockTypeNode>(nodes);
        }

        internal void Add(TBlockTypeNode node) =>
            _nodes.Add(node);

        /// <summary>
        /// Enumerates <see cref="Nodes"/>.
        /// </summary>
        public IEnumerator<TBlockTypeNode> GetEnumerator() => Nodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private string GetDebuggerDisplay() =>
            $"[{Context.ContextType}, {Context.VisitedType.Name}, Nodes = {Nodes.Count}]";
    }
}
