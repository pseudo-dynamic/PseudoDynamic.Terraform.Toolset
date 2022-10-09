using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class TerraformDefinitionVisitor
    {
        public virtual void Visit(TerraformDefinition definition) =>
            definition.Visit(this);

        protected internal virtual void VisitPrimitive(PrimitiveDefinition definition)
        {
        }

        protected internal virtual void VisitMonoRange(MonoRangeDefinition definition) =>
            Visit(definition.Item);

        protected internal virtual void VisitMap(MapDefinition definition)
        {
            Visit(definition.Key);
            Visit(definition.Value);
        }

        protected internal virtual void VisitObject(ObjectDefinition definition)
        {
            foreach (var attribute in definition.Attributes) {
                Visit(attribute);
            }
        }

        protected internal virtual void VisitObjectAttribute(ObjectAttributeDefinition definition) =>
            Visit(definition.Value);

        protected internal virtual void VisitTuple(TupleDefinition definition)
        {
        }

        protected internal virtual void VisitBlock(BlockDefinition definition)
        {
            foreach (var attribute in definition.Attributes) {
                Visit(attribute);
            }
        }

        protected internal virtual void VisitBlockAttribute(BlockAttributeDefinition definition) =>
            Visit(definition.Value);
    }
}
