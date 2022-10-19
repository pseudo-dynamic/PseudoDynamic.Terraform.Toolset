using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class AttributeReflectionMetadata
    {
        public PropertyInfo Property { get; }

        public AttributeReflectionMetadata(PropertyInfo property) =>
            Property = property ?? throw new ArgumentNullException(nameof(property));
    }
}
