namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal sealed record class PrimitiveDefinition : ValueDefinition
    {
        public static readonly PrimitiveDefinition String = new(typeof(string), TerraformTypeConstraint.String);
        public static readonly PrimitiveDefinition Number = new(typeof(int), TerraformTypeConstraint.Number);
        public static readonly PrimitiveDefinition Bool = new(typeof(bool), TerraformTypeConstraint.Bool);
        public static readonly PrimitiveDefinition Any = new(typeof(object), TerraformTypeConstraint.Dynamic);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Primitive;

        public override TerraformTypeConstraint TypeConstraint { get; }

        public DefaultBuilder With => new DefaultBuilder(TypeConstraint);

        internal PrimitiveDefinition(Type sourceType, TerraformTypeConstraint typeConstraint)
            : base(sourceType) =>
            TypeConstraint = typeConstraint;

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitPrimitive(this);

        public class DefaultBuilder
        {
            public TerraformTypeConstraint _typeConstraint;

            public DefaultBuilder(TerraformTypeConstraint typeConstraint) =>
                _typeConstraint = typeConstraint;

            public PrimitiveDefinition Build(Type sourceType) =>
                new(sourceType, _typeConstraint);
        }
    }
}
