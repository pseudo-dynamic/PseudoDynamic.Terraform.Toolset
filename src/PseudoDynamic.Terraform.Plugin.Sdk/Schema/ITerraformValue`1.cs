using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema
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
    [TypeConstraintEvaluationPrevention]
    public interface ITerraformValue<[TerraformValueType] out T> : ITerraformValue
    {
        /// <summary>
        /// The deserialized Terraform value.
        /// </summary>
        new T Value { get; }

        object? ITerraformValue.Value => Value;

        /// <summary>
        /// True means <see cref="Value"/> is null.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        new bool IsNull { get; }

        bool ITerraformValue.IsNull => IsNull;

        /// <summary>
        /// True indicates, that <see cref="Value"/> is not yet known.
        /// </summary>
        new bool IsUnknown { get; }

        bool ITerraformValue.IsUnknown => IsUnknown;
    }
}
