using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal record class NestedBlockAttributeDefinition : BlockAttributeDefinitionBase
    {
        public const int DefaultMinimumItems = 0;
        public const int DefaultMaximumItems = 0;

        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.NestedBlockAttribute;

        public override ValueDefinition Value {
            get => base.Value;

            internal init {
                base.Value = value;
                ParseValue(value);
            }
        }

        public BlockDefinition Block { get; private set; }

        public int MinimumItems { get; init; } = DefaultMinimumItems;

        public int MaximumItems { get; init; } = DefaultMaximumItems;

        internal NestedBlockAttributeDefinition(BlockAttributeDefinition blockAttribute)
            : base(blockAttribute) =>
            ParseValue(blockAttribute.Value);

        private NestedBlockAttributeDefinition(string name, ValueDefinition value)
            : base(name, value) =>
            ParseValue(value);

        public NestedBlockAttributeDefinition(string name, RangeDefinition value)
            : this(name, (ValueDefinition)value)
        {
        }

        public NestedBlockAttributeDefinition(string name, BlockDefinition value)
            : this(name, (ValueDefinition)value)
        {
        }

        [MemberNotNull(nameof(Block))]
        private void ParseValue(ValueDefinition value)
        {
            ValueDefinition block;

            if (value is INestedValueAccessor nestedValueAccessor) {
                block = nestedValueAccessor.NestedValue;
            } else {
                block = value;
            }

            if (block is not BlockDefinition typedBlock) {
                throw new NestedBlockException($"The nested value was expected to be a nested block but it was {block.TypeConstraint}");
            }

            Block = typedBlock;
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitNestedBlock(this);

        public virtual bool Equals(NestedBlockAttributeDefinition? definition) =>
            definition is not null
            && ValueWrapping == definition.ValueWrapping
            && MinimumItems == definition.MinimumItems
            && MaximumItems == definition.MaximumItems;
    }
}
