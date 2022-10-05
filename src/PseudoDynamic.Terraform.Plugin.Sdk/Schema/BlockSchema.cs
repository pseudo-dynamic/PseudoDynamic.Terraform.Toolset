namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal sealed record BlockSchema
    {
        public static readonly BlockSchema Empty = new BlockSchema(new Dictionary<string, AttributeSchema>());

        public IReadOnlyDictionary<string, AttributeSchema> Attributes => _attributes;

        private Dictionary<string, AttributeSchema> _attributes;

        public BlockSchema(BlockSchema template) =>
            _attributes = new Dictionary<string, AttributeSchema>(template.Attributes);

        public BlockSchema(Dictionary<string, AttributeSchema> attributes) =>
            _attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
    }
}
