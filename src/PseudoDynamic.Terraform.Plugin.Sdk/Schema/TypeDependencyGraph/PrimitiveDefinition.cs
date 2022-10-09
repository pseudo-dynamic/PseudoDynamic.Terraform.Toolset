namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class PrimitiveDefinition : ValueDefinition
    {
        public static readonly PrimitiveDefinition String = new PrimitiveDefinition(TerraformTypeConstraint.String);
        public static readonly PrimitiveDefinition Number = new PrimitiveDefinition(TerraformTypeConstraint.Number);
        public static readonly PrimitiveDefinition Bool = new PrimitiveDefinition(TerraformTypeConstraint.Bool);
        public static readonly PrimitiveDefinition Any = new PrimitiveDefinition(TerraformTypeConstraint.Any);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Primitive;

        public override TerraformTypeConstraint TypeConstraint { get; }

        internal PrimitiveDefinition(TerraformTypeConstraint typeConstraint) =>
            TypeConstraint = typeConstraint;

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitPrimitive(this);
    }
}
