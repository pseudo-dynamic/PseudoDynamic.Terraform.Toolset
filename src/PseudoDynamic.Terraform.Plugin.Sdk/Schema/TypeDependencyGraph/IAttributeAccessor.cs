namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal interface IAttributeAccessor
    {
        int Count { get; }

        AttributeDefinition GetAttribute(string attributeName);

        IEnumerable<AttributeDefinition> GetEnumerator();
    }
}
