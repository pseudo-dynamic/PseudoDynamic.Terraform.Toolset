﻿namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class BlockDefinition : ComplexDefinition, IAttributeAccessor
    {
        internal static readonly BlockDefinition Uncomputed = new(new BlockDefinition(UncomputedSourceType));

        internal const int DefaultVersion = 1;

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Block;

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Block;

        public int Version { get; init; } = DefaultVersion;

        public IReadOnlyList<BlockAttributeDefinition> Attributes {
            get => _attributes;

            init {
                PreventAttributeDuplicates(value, _blocks);
                _attributes = value;
            }
        }

        public IReadOnlyList<NestedBlockAttributeDefinition> Blocks {
            get => _blocks;

            init {
                PreventAttributeDuplicates(_attributes, value);
                _blocks = value;
            }
        }

        int IAttributeAccessor.Count => Attributes.Count + Blocks.Count;

        IEnumerable<AttributeDefinition> IAttributeAccessor.GetEnumerator() =>
            ((IEnumerable<AttributeDefinition>)Attributes).Concat(Blocks);

        /// <summary>
        /// The description of this attribute.
        /// </summary>
        public string Description {
            get => _description;
            init => _description = value ?? string.Empty;
        }

        /// <summary>
        /// Kind of description.
        /// </summary>
        public DescriptionKind DescriptionKind { get; init; }

        /// <summary>
        /// Marks this block as deprecated.
        /// </summary>
        public bool IsDeprecated { get; init; }

        private string _description = string.Empty;
        private IReadOnlyList<BlockAttributeDefinition> _attributes = Array.Empty<BlockAttributeDefinition>();
        private IReadOnlyDictionary<string, int>? _indexedAttributes;
        private IReadOnlyList<NestedBlockAttributeDefinition> _blocks = Array.Empty<NestedBlockAttributeDefinition>();
        private IReadOnlyDictionary<string, int>? _indexedBlocks;

        public BlockDefinition(Type sourceType) : base(sourceType)
        {
        }

        private void PreventAttributeDuplicates(IReadOnlyList<AttributeDefinition> attributes, IReadOnlyList<AttributeDefinition> blocks)
        {
            var attributesCount = attributes.Count;
            var blocksCount = blocks.Count;

            if (attributesCount == 0 && blocksCount == 0) {
                return;
            }

            var havingAttributes = attributesCount > 0;
            var havingBlocks = blocksCount > 0;

            if (havingAttributes) {
                PreventAttributeDuplicates(attributes, nameof(Attributes));
            }

            if (havingBlocks) {
                PreventAttributeDuplicates(blocks, nameof(Blocks));
            }

            if (havingAttributes && havingBlocks) {
                PreventAttributeDuplicates(attributes.Concat(blocks), static data => $"The block {data.AdditionalData.FullName} cannot have a nested block and an attribute with the same name: {data.AttributeName}", SourceType);
            }
        }

        AttributeDefinition IAttributeAccessor.GetAttribute(string attributeName)
        {
            _indexedAttributes ??= ToDictionary(_attributes);

            if (_indexedAttributes.TryGetValue(attributeName, out var attributeIndex)) {
                return _attributes[attributeIndex];
            }

            _indexedBlocks ??= ToDictionary(_blocks);

            if (_indexedBlocks.TryGetValue(attributeName, out attributeIndex)) {
                return _blocks[attributeIndex];
            }

            throw new BlockException($"The block {SourceType.FullName} does not have a nested block and an attribute with the name \"{attributeName}\"");

            static IReadOnlyDictionary<string, int> ToDictionary(IReadOnlyList<AttributeDefinition> attributes)
            {
                var attributesCount = attributes.Count;
                var dictionary = new Dictionary<string, int>();

                for (var i = 0; i < attributesCount; i++) {
                    dictionary.Add(attributes[i].Name, i);
                }

                return dictionary;
            }
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitBlock(this);

        public virtual bool Equals(BlockDefinition? other) =>
            other is not null
            && base.Equals(other)
            && Version == other.Version
            && Description == other.Description
            && DescriptionKind == other.DescriptionKind
            && IsDeprecated == other.IsDeprecated;

        public override int GetHashCode() => HashCode.Combine(
            base.GetHashCode(),
            Version,
            Description,
            DescriptionKind,
            IsDeprecated);
    }
}
