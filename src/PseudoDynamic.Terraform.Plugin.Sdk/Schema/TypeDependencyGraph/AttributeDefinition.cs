using System.Text;

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

        public virtual ValueDefinition Value {
            get => _value ?? throw new InvalidOperationException("Value has been not set");

            internal init {
                _value = value;
                ParseValue(value);
            }
        }

        public ValueWrapping? ValueWrapping { get; private set; }

        public AttributeReflectionMetadata AttributeReflectionMetadata {
            get => _attributeReflectionMetadata ?? throw new InvalidOperationException("Attribute reflection metadata has not been set");
            init => _attributeReflectionMetadata = value;
        }

        private ValueDefinition? _value;
        private bool _isOptional;
        private bool _isRequired = DefaultIsRequired;
        private AttributeReflectionMetadata? _attributeReflectionMetadata;

        protected AttributeDefinition(Type sourceType, string name, ValueDefinition value)
            : base(sourceType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _value = value ?? throw new ArgumentNullException(nameof(value));
            ParseValue(value);
        }

        private void ParseValue(ValueDefinition value)
        {
            if (value is INestedValueProvider) {
                ValueWrapping = value.TypeConstraint.ToValueWrapping();
            }
        }

        public virtual bool Equals(AttributeDefinition? other) =>
            other is not null
            && base.Equals(other)
            && Name == other.Name
            && IsRequired == other.IsRequired
            && IsOptional == other.IsOptional
            && ValueWrapping == other.ValueWrapping;

        public override int GetHashCode() => HashCode.Combine(
            base.GetHashCode(),
            Name,
            IsRequired,
            IsOptional,
            ValueWrapping);

        protected override bool PrintMembers(StringBuilder builder) =>
            base.PrintMembers(builder.Append($"{new {
                Name,
                IsRequired,
                IsOptional,
                ValueWrapping
            }.ToString()?.Trim('{', '}')}, "));
    }
}
