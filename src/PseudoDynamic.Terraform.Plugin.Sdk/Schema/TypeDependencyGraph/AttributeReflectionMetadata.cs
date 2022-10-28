using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class AttributeReflectionMetadata
    {
        public AttributeReflectionMetadata Uncomputed() => new();

        public PropertyInfo Property { get; private set; }

        private AttributeReflectionMetadata() => Property = null!;

        public AttributeReflectionMetadata(PropertyInfo property) =>
            Property = property ?? throw new ArgumentNullException(nameof(property));
    }
}
