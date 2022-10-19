namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal interface IAbstractAttributeAccessor
    {
        public AttributeDefinition GetAbstractAttribute(string attributeName);
    }
}
