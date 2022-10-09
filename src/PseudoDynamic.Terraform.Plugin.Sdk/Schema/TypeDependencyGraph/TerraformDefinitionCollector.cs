using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
