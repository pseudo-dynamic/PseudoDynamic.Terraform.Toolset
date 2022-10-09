namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class BlockDefinition : ComplexDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Block; 
        
        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Block;

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitBlock(this);
    }
}
