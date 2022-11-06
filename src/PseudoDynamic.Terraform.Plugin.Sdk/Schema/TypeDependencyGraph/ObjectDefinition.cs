namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class ObjectDefinition : ComplexDefinition, IAttributeAccessor
    {
        public static ObjectDefinition Uncomputed() => new(UncomputedSourceType);

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Object;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Object;

        public IReadOnlyList<ObjectAttributeDefinition> Attributes {
            get => _attributes;

            init {
                PreventAttributeDuplicates(value, nameof(Attributes));
                _attributes = value;
            }
        }

        int IAttributeAccessor.Count => Attributes.Count;

        IEnumerable<AttributeDefinition> IAttributeAccessor.GetEnumerator() => Attributes;

        private IReadOnlyList<ObjectAttributeDefinition> _attributes = Array.Empty<ObjectAttributeDefinition>();
        private IReadOnlyDictionary<string, int>? _indexedAttributes;

        public ObjectDefinition(Type sourceType) : base(sourceType)
        {
        }

        AttributeDefinition IAttributeAccessor.GetAttribute(string attributeName)
        {
            _indexedAttributes ??= ToDictionary(_attributes);

            if (_indexedAttributes.TryGetValue(attributeName, out var attributeIndex)) {
                return _attributes[attributeIndex];
            }

            throw new BlockException($"The object {SourceType.FullName} does not have an attribute with the name \"{attributeName}\"");
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitObject(this);

        public virtual bool Equals(ObjectDefinition? other) => base.Equals(other);

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
