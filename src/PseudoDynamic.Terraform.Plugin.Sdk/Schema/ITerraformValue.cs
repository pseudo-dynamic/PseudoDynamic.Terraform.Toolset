using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Represents a value that follows the conventions of a value of Terraform,
    /// that can be null or unknown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// If you model a schema, and you want to have access to <see cref="IsNull"/>
    /// or <see cref="IsUnknown"/> of a value, then you can use
    /// <see cref="ITerraformValue{T}"/> or <see cref="TerraformValue{T}"/>
    /// as property type or a other possible derivation of
    /// <see cref="ITerraformValue{T}"/>. Keep in mind to use
    /// <see cref="TerraformValueTypeAttribute"/>.
    /// </remarks>
    public interface ITerraformValue<[TerraformValueType] out T>
    {
        /// <summary>
        /// The deserialized Terraform value.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// True means <see cref="Value"/> is null.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        bool IsNull { get; }

        /// <summary>
        /// True indicates, that <see cref="Value"/> is not yet known.
        /// </summary>
        bool IsUnknown { get; }
    }
}
