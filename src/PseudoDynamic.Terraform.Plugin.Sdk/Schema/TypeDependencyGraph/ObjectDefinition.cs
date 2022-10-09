namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class ObjectDefinition : ComplexDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Object;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Object;

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitObject(this);
    }
}
