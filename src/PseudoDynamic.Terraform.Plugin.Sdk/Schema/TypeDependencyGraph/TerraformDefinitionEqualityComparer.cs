using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class TerraformDefinitionEqualityComparer : EqualityComparer<TerraformDefinition>
    {
        public new static readonly TerraformDefinitionEqualityComparer Default = new TerraformDefinitionEqualityComparer();

        public override bool Equals(TerraformDefinition? x, TerraformDefinition? y)
        {
            if (ReferenceEquals(x, y)) {
                return true;
            }

            if (x is null || y is null) {
                return false;
            }

            var definitions = TerraformDefinitionCollector.Default.Queue(x);
            var definitionVisitor = new EqualityComparingVisitor(definitions);
            definitionVisitor.Visit(y);
            return definitionVisitor.AreEqual;
        }

        public override int GetHashCode([DisallowNull] TerraformDefinition obj) => throw new NotImplementedException();

        private class EqualityComparingVisitor : TerraformDefinitionVisitor
        {
            public bool AreEqual => _areEqual;

            private bool _areEqual = true;

            private readonly Queue<TerraformDefinition> _queue;

            public EqualityComparingVisitor(Queue<TerraformDefinition> queue) =>
                _queue = queue;

            public override void Visit(TerraformDefinition y)
            {
                if (!_queue.TryDequeue(out var x) || !x.Equals(y)) {
                    _areEqual = false;
                    return;
                }

                base.Visit(y);
            }
        }
    }
}
