using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Represents the value in the view of Terraform where it can be null or unknown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed record TerraformValue<[TerraformValueType] T> : ITerraformValue<T>
    {
        /// <summary>
        /// The value.
        /// </summary>
        public T Value {
            get {
                var isNullable = _isNullable;
                var value = _value;

                if (!isNullable && Equals(value, default)) {
                    throw new TerraformValueException("The value type indicates non-nullability but value is null");
                }

                return value!; // If the user has created an instance manually we do not enforce nullability check
            }

            init => _value = value;
        }

        /// <summary>
        /// Specifies that the value is null.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        public bool IsNull { get; init; }

        /// <summary>
        /// Specifies that the value is unknown, but known at apply-time.
        /// </summary>
        public bool IsUnknown { get; init; }

        private bool _isNullable;
        private T? _value;

        /// <summary>
        /// Creates an immutable Terraform value.
        /// </summary>
        public TerraformValue()
        {
        }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="isNullable">A hint whether value is nullable.</param>
        /// <param name="value"></param>
        /// <param name="isNull"></param>
        /// <param name="isUnknown"></param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection")]
        private TerraformValue(bool isNullable, T? value, bool isNull, bool isUnknown)
        {
            _isNullable = isNullable;
            _value = value;
            IsNull = isNull;
            IsUnknown = isUnknown;
        }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="isNullable">A hint whether value is nullable.</param>
        internal TerraformValue(bool isNullable) => _isNullable = isNullable;
    }
}
