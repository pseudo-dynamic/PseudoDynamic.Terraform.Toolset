namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines an attribute of one block.
    /// </summary>
    internal record class BlockAttributeDefinition : AttributeDefinition
    {
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.BlockAttribute;

        /// <summary>
        /// The description of this attribute.
        /// </summary>
        public string Description {
            get => _description;
            init => _description = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// The value this attribute describes will be computed.
        /// </summary>
        public bool IsComputed { get; init; }

        /// <summary>
        /// Marks this attribute as sensitive.
        /// </summary>
        public bool IsSensitive { get; init; }

        /// <summary>
        /// Kind of description.
        /// </summary>
        public DescriptionKind DescriptionKind { get; init; }

        /// <summary>
        /// Marks this attribute as deprecated.
        /// </summary>
        public bool IsDeprecated { get; init; }

        private string _description = string.Empty;

        //internal BlockAttributeDefinition(string name)
        //    : base(name)
        //{
        //}

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BlockAttributeDefinition(string name, ValueDefinition value)
            : base(name, value)
        {
        }

        protected internal override void Visit(TerraformDefinitionVisitor visitor) =>
            visitor.VisitBlockAttribute(this);
    }
}
