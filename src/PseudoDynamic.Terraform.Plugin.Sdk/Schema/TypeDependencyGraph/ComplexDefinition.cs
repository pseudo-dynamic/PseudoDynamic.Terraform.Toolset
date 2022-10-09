using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class ComplexDefinition : ValueDefinition
    {
        public IReadOnlyList<AttributeDefinition> Attributes { get; init; } = Array.Empty<AttributeDefinition>();

        public virtual bool Equals(ComplexDefinition? other) => base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
