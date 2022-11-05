using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin
{
    /// <summary>
    /// Represents a value that follows the conventions of a value of Terraform, that can be null or unknown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// If you model a schema, and you want to have access to <see cref="IsNull"/> or <see cref="IsUnknown"/>
    /// of a value, then you can use <see cref="ITerraformValue{T}"/> or <see cref="TerraformValue{T}"/> to
    /// wrap any type, that is representable by a Terraform type constraint.
    /// </remarks>
    [SkipImplicitTypeConstraintEvaluation]
    public sealed class TerraformValue<[TerraformValueType] T> : ITerraformValue<T>, IEquatable<TerraformValue<T>>
    {
        /// <summary>
        /// Representing a Terraform null.
        /// </summary>
        public static readonly TerraformValue<T> Null = new TerraformValue<T>();

        /// <summary>
        /// Representing a Terraform unknown.
        /// </summary>
        public static readonly TerraformValue<T> Unknown = new TerraformValue<T>() { IsUnknown = true };

        /// <summary>
        /// The value.
        /// </summary>
        public T Value {
            get {
                var isNullable = _isNullable;
                var value = _value;

                if (isNullable.HasValue && !isNullable.Value && Equals(value, default)) {
                    throw new TerraformValueException("The value type indicates non-nullability but value is null");
                }

                return value!; // If the user has created an instance manually we do not enforce nullability check
            }

            private init {
                _value = value;

                // For the case value is reference or Nullable<>
                // type and its HasValue is false.
                if (ReferenceEquals(value, null)) {
                    _isNotNull = false;
                }
                // We only know when value is not equals default,
                // that the value canont be not null.
                else if (!Equals(value, default)) {
                    _isNotNull = true;
                    _isUnknown = false;
                }
            }
        }

        object? ITerraformValue.Value => Value;

        /// <summary>
        /// Specifies that the value is null.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        public bool IsNull {
            get => !_isNotNull;

            private init {
                _isNotNull = !value;

                // if null, then we set default value
                if (!value) {
                    _value = default;
                }
            }
        }

        /// <summary>
        /// A shortcut for <see cref="TerraformValue{T}.AsNull"/>.
        /// </summary>
        public TerraformValue<T> AsNull => Null;
        ITerraformValue<T> ITerraformValue<T>.AsNull => AsNull;

        /// <summary>
        /// Specifies that the value is unknown, but known at apply-time.
        /// </summary>
        public bool IsUnknown {
            get => _isUnknown;

            private init {
                _isUnknown = value;

                if (value) {
                    _value = default;
                    _isNotNull = false;
                }
            }
        }

        /// <summary>
        /// A shortcut for <see cref="TerraformValue{T}.Unknown"/>.
        /// </summary>
        public TerraformValue<T> AsUnknown => Unknown;
        ITerraformValue<T> ITerraformValue<T>.AsUnknown => AsUnknown;

        private bool? _isNullable;
        private bool _isNotNull;
        private T? _value;
        private bool _isUnknown;

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
            _isNotNull = !isNull;
            _isUnknown = isUnknown;
        }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="isNullable">A hint whether value is nullable.</param>
        internal TerraformValue(bool isNullable) => _isNullable = isNullable;

        /// <summary>
        /// Creates a Terraform null value.
        /// </summary>
        public TerraformValue()
        {
        }

        /// <summary>
        /// Creates a Terraform value of <paramref name="value"/>, that can result into a Terraform null value if <paramref name="value"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="value"></param>
        public TerraformValue(T value) => Value = value;

        private bool Equals(ITerraformValue<T> other) =>
            EqualityComparer<T>.Default.Equals(_value, other.Value)
            && !_isNotNull == other.IsNull
            && _isUnknown == other.IsUnknown;

        /// <inheritdoc/>
        public bool Equals(TerraformValue<T>? other) =>
            other is not null
            && Equals((ITerraformValue<T>)other);

        /// <inheritdoc/>
        public override bool Equals(object? other) =>
            other is ITerraformValue<T> typedOther
            && Equals(typedOther);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(
            _value,
            !_isNotNull,
            _isUnknown);

        public static implicit operator TerraformValue<T>(T value) =>
            new TerraformValue<T>(value);
    }
}
