namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class TerraformDefinitionEqualityComparer : EqualityComparer<TerraformDefinition>
    {
        public new static readonly TerraformDefinitionEqualityComparer Default = new(static (x, y) => object.Equals(x, y));

        private readonly Func<TerraformDefinition?, TerraformDefinition?, bool> _checkEquality;

        public TerraformDefinitionEqualityComparer(Func<TerraformDefinition?, TerraformDefinition?, bool> checkEquality) =>
            _checkEquality = checkEquality;

        public override bool Equals(TerraformDefinition? x, TerraformDefinition? y)
        {
            if (ReferenceEquals(x, y)) {
                return true;
            }

            if (x is null || y is null) {
                return false;
            }

            Queue<TerraformDefinition> queueOfX = TerraformDefinitionCollector.Default.Queue(x);
            EqualityComparingVisitor definitionVisitor = new(queueOfX, _checkEquality);
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

            private readonly Queue<TerraformDefinition> _queueOfX;
            private readonly Func<TerraformDefinition?, TerraformDefinition?, bool> _checkEquality;
            private readonly int _queueCount;

            public EqualityComparingVisitor(Queue<TerraformDefinition> queueOfX, Func<TerraformDefinition?, TerraformDefinition?, bool> checkEquality)
            {
                _queueOfX = queueOfX ?? throw new ArgumentNullException(nameof(queueOfX));
                _checkEquality = checkEquality ?? throw new ArgumentNullException(nameof(checkEquality));
                _queueCount = queueOfX.Count;
            }

            public override void Visit(TerraformDefinition y)
            {
                if (_onceVisited) {
                    throw new InvalidOperationException("This instance is stateful and cannot be used to re-compare another definition");
                }

                bool dequeueSucceeded = _queueOfX.TryDequeue(out TerraformDefinition? x);

                if (dequeueSucceeded) {
                    _visitCounter++;
                    _areEqual = _checkEquality(x, y);
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
