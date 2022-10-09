namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class TupleDefinition : RangeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Tuple;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Tuple;

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitTuple(this);
    }
}
