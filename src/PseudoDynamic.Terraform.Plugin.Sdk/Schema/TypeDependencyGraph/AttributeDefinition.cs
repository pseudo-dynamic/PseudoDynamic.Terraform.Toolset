namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class AttributeDefinition : TerraformDefinition
    {
        protected internal const bool DefaultIsRequired = true;

        /// <summary>
        /// The attribute name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value this attribute describes is required. (default true)
        /// </summary>
        public virtual bool IsRequired {
            get => _isRequired;
            init {
                _isRequired = value;
                _isOptional = !value;
            }
        }

        /// <summary>
        /// The value this attribute describes is optional.
        /// </summary>
        public virtual bool IsOptional {
            get => _isOptional;

            init {
                _isOptional = value;
                _isRequired = !value;
            }
        }

        public ValueDefinition Value {
            get => _value ?? throw new InvalidOperationException("Value has been not set");
            internal init => _value = value;
        }

        private ValueDefinition? _value;
        private bool _isOptional;
        private bool _isRequired = DefaultIsRequired;

        protected AttributeDefinition(string name, ValueDefinition value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public virtual bool Equals(AttributeDefinition? other) =>
            other is not null
            && base.Equals(other)
            && Name == other.Name
            && IsRequired == other.IsRequired
            && IsOptional == other.IsOptional;

        public override int GetHashCode() => HashCode.Combine(
            base.GetHashCode(),
            Name,
            IsRequired,
            IsOptional);
    }
}
