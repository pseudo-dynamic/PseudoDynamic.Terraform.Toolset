namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class ObjectAttributeDefinition : AttributeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.ObjectAttribute;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ObjectAttributeDefinition(string name, ValueDefinition value)
            : base(name, value)
        {
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitObjectAttribute(this);
    }
}
