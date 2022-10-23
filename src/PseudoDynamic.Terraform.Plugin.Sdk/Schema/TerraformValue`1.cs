﻿using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Represents the value in the view of Terraform where it can be null or unknown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [TypeConstraintEvaluationPrevention]
    public sealed class TerraformValue<[TerraformValueType] T> : ITerraformValue<T>, IEquatable<TerraformValue<T>>
    {
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

            init {
                _value = value;

                // We only know when value is not equals default,
                // that the value canont be not null.
                if (!Equals(value, default)) {
                    _isNotNull = true;
                    _isUnknown = false;
                }
            }
        }

        /// <summary>
        /// Specifies that the value is null.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        public bool IsNull {
            get => !_isNotNull;

            init {
                _isNotNull = !value;

                // if null, then we set default value
                if (!value) {
                    _value = default;
                }
            }
        }

        /// <summary>
        /// Specifies that the value is unknown, but known at apply-time.
        /// </summary>
        public bool IsUnknown {
            get => _isUnknown;

            init {
                _isUnknown = value;

                if (value) {
                    _value = default;
                    _isNotNull = false;
                }
            }
        }

        private bool? _isNullable;
        private bool _isNotNull;
        private T? _value;
        private bool _isUnknown;

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
            _isNotNull = !isNull;
            _isUnknown = isUnknown;
        }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="isNullable">A hint whether value is nullable.</param>
        internal TerraformValue(bool isNullable) => _isNullable = isNullable;

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
    }
}