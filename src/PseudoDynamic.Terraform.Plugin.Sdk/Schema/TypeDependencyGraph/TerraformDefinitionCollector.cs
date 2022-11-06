namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class TerraformDefinitionCollector
    {
        public static readonly TerraformDefinitionCollector Default = new();

        public Queue<TerraformDefinition> Queue(TerraformDefinition definition)
        {
            QueuingVisitor visitor = new();
            visitor.Visit(definition);
            return visitor.Queue;
        }

        public Stack<TerraformDefinition> Stack(TerraformDefinition definition)
        {
            StackingVisitor visitor = new();
            visitor.Visit(definition);
            return visitor.Stack;
        }

        private class QueuingVisitor : TerraformDefinitionVisitor
        {
            public Queue<TerraformDefinition> Queue { get; } = new();

            public override void Visit(TerraformDefinition definition)
            {
                Queue.Enqueue(definition);
                base.Visit(definition);
            }
        }

        private class StackingVisitor : TerraformDefinitionVisitor
        {
            public Stack<TerraformDefinition> Stack { get; } = new();

            public override void Visit(TerraformDefinition definition)
            {
                Stack.Push(definition);
                base.Visit(definition);
            }
        }
    }
}
