using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Types
{
    /// <summary>
    /// Represents the value in the view of Terraform where it can be null or unknown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record TerraformValue<T> : ITerraformValue<T>
    {
        /// <summary>
        /// The value.
        /// </summary>
        public T Value {
            get {
                var isNullable = _isNullable;
                var value = _value;

                if (!isNullable && value == null) {
                    throw new InvalidOperationException("The value type indicates non-nullability but value is null");
                }

                return value;
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
        private T _value = default!;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="isNullable">A hint whether value is nullable.</param>
        internal TerraformValue(bool isNullable) => _isNullable = isNullable;
    }
}
