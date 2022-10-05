using System.Collections;
using System.Diagnostics;
using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class SchemaTypeNode : IEnumerable<SchemaTypeNode>
    {
        internal VisitingContext Context { get; }

        public IReadOnlyList<SchemaTypeNode> Nodes => _nodes;

        private readonly List<SchemaTypeNode> _nodes;

        internal SchemaTypeNode(VisitingContext context)
        {
            Context = context;
            _nodes = new List<SchemaTypeNode>();
        }

        internal SchemaTypeNode(VisitingContext context, IEnumerable<SchemaTypeNode> nodes)
        {
            Context = context;
            _nodes = new List<SchemaTypeNode>(nodes);
        }

        internal void Add(SchemaTypeNode node) =>
            _nodes.Add(node);

        /// <summary>
        /// Enumerates <see cref="Nodes"/>.
        /// </summary>
        public IEnumerator<SchemaTypeNode> GetEnumerator() => Nodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private string GetDebuggerDisplay() =>
            $"[{Context.ContextType}, {Context.VisitingType.Type.Name}, Nodes = {Nodes.Count}]";
    }
}
