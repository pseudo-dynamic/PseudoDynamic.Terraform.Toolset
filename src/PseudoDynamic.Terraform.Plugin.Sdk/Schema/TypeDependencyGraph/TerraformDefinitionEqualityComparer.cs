using System.Diagnostics.CodeAnalysis;

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
            public bool AreEqual => _areEqual && _queueCount == _visitCounter;

            private int _visitCounter;
            private bool _areEqual = true;
            private bool _onceVisited;

            private readonly Queue<TerraformDefinition> _queue;
            private readonly int _queueCount;

            public EqualityComparingVisitor(Queue<TerraformDefinition> queue)
            {
                _queue = queue ?? throw new ArgumentNullException(nameof(queue));
                _queueCount = queue.Count;
            }

            public override void Visit(TerraformDefinition y)
            {
                if (_onceVisited) {
                    throw new InvalidOperationException("This instance is stateful and cannot be used to re-compare another definition");
                }

                bool dequeueSucceeded = _queue.TryDequeue(out var x);

                if (dequeueSucceeded) {
                    _visitCounter++;
                    _areEqual = x!.Equals(y);
                }

                if (!dequeueSucceeded || !_areEqual) {
                    _onceVisited = true;
                    return;
                }

                base.Visit(y);
            }
        }
    }
}
