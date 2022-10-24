using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Describes more relevant reflection metadata for <see cref="ComplexDefinition"/>.
    /// </summary>
    internal record class ComplexReflectionMetadata : ComplexTypeMetadata
    {
        public IReadOnlyDictionary<string, string> PropertyNameAttributeNameMapping { get; }

        public ComplexReflectionMetadata(
            ComplexTypeMetadata complexTypeMetadata,
            IReadOnlyDictionary<string, string> propertyNameAttributeNameMapping)
            : base(complexTypeMetadata) =>
            PropertyNameAttributeNameMapping = propertyNameAttributeNameMapping ?? throw new ArgumentNullException(nameof(propertyNameAttributeNameMapping));
    }
}
