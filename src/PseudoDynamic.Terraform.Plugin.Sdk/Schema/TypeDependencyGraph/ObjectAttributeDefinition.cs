namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class ObjectAttributeDefinition : AttributeDefinition
    {
        public static ObjectAttributeDefinition Uncomputed(string name, ValueDefinition value) =>
            new ObjectAttributeDefinition(UncomputedSourceType, name, value);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.ObjectAttribute;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ObjectAttributeDefinition(Type sourceType, string name, ValueDefinition value)
            : base(sourceType, name, value)
        {
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitObjectAttribute(this);
    }
}
