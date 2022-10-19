namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class TupleDefinition : RangeDefinition
    {
        public static TupleDefinition Uncomputed() =>
            new TupleDefinition(UncomputedSourceType);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Tuple;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Tuple;

        public TupleDefinition(Type sourceType) : base(sourceType)
        {
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitTuple(this);
    }
}
