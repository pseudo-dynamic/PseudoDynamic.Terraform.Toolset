namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class TerraformDefinitionCollector
    {
        public static readonly TerraformDefinitionCollector Default = new TerraformDefinitionCollector();

        public Queue<TerraformDefinition> Queue(TerraformDefinition definition)
        {
            var visitor = new QueuingVisitor();
            visitor.Visit(definition);
            return visitor.Queue;
        }

        public Stack<TerraformDefinition> Stack(TerraformDefinition definition)
        {
            var visitor = new StackingVisitor();
            visitor.Visit(definition);
            return visitor.Stack;
        }

        private class QueuingVisitor : TerraformDefinitionVisitor
        {
            public Queue<TerraformDefinition> Queue => _queue;

            private Queue<TerraformDefinition> _queue = new Queue<TerraformDefinition>();

            public override void Visit(TerraformDefinition definition)
            {
                _queue.Enqueue(definition);
                base.Visit(definition);
            }
        }

        private class StackingVisitor : TerraformDefinitionVisitor
        {
            public Stack<TerraformDefinition> Stack => _stack;

            private Stack<TerraformDefinition> _stack = new Stack<TerraformDefinition>();

            public override void Visit(TerraformDefinition definition)
            {
                _stack.Push(definition);
                base.Visit(definition);
            }
        }
    }
}
