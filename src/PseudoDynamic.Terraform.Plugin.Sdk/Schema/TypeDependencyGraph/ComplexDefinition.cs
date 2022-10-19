using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class ComplexDefinition : ValueDefinition
    {
        protected static IReadOnlyDictionary<string, int> ToDictionary(IReadOnlyList<AttributeDefinition> attributes)
        {
            var attributesCount = attributes.Count;
            var dictionary = new Dictionary<string, int>();

            for (int i = 0; i < attributesCount; i++) {
                dictionary.Add(attributes[i].Name, i);
            }

            return dictionary;
        }

        public ComplexReflectionMetadata ComplexReflectionMetadata {
            get => _complexMetadata ?? throw new InvalidOperationException("Complex reflection metadata has not been set");
            init => _complexMetadata = value;
        }

        private ComplexReflectionMetadata? _complexMetadata;

        protected ComplexDefinition(Type sourceType) : base(sourceType)
        {
        }

        protected void PreventAttributeDuplicates<T>(IEnumerable<AttributeDefinition> attributes, Func<(string AttributeName, T AdditionalData), string> getErrorMessage, T? additionalData = default)
        {
            var attributeNameSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var attribute in attributes) {
                var attributeName = attribute.Name;

                if (attributeNameSet.Contains(attributeName)) {
                    throw new BlockException(getErrorMessage((attributeName, additionalData!)));
                }
            }
        }

        protected void PreventAttributeDuplicates(IEnumerable<AttributeDefinition> attributes, Func<(string AttributeName, object? AdditionalData), string> getErrorMessage) =>
            PreventAttributeDuplicates(attributes, getErrorMessage, additionalData: default);

        protected void PreventAttributeDuplicates(IEnumerable<AttributeDefinition> attributes, string attributesPropertyName) =>
            PreventAttributeDuplicates(
                attributes,
                static data => $"Block's property {data.AdditionalData.InstanceType.GetProperty(data.AdditionalData.AttributesPropertyName, BindingFlags.Instance | BindingFlags.Public)?.GetPath()} contains an attribute duplicate: {data.AttributeName}",
                (InstanceType: GetType(), AttributesPropertyName: attributesPropertyName));

        public virtual bool Equals(ComplexDefinition? other) => base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
