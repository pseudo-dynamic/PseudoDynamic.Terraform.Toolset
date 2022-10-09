using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class AttributeDefinition : TerraformDefinition
    {
        /// <summary>
        /// The attribute name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value this attribute describes is required.
        /// </summary>
        public bool IsRequired { get; init; }

        /// <summary>
        /// The value this attribute describes is optional.
        /// </summary>
        public bool IsOptional { get; init; }

        public ValueDefinition Value {
            get => _value ?? throw new InvalidOperationException("Value has been not set");
            internal init => _value = value;
        }

        private ValueDefinition? _value;

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
