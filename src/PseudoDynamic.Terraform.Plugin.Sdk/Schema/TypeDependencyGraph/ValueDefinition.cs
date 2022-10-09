using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class ValueDefinition : TerraformDefinition
    {
        public bool IsWrappedByTerraformValue { get; init; }

        public abstract TerraformTypeConstraint TypeConstraint { get; }
    }
}
