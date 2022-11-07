using MessagePack;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

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
    [SkipImplicitTypeConstraintEvaluation]
    public interface ITerraformValue<[TerraformValueType] out T> : ITerraformValue
    {
        /// <summary>
        /// The deserialized Terraform value.
        /// </summary>
        new T Value { get; }

        object? ITerraformValue.Value => Value;

        /// <summary>
        /// <see langword="true"/> means <see cref="Value"/> is <see langword="null"/>.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        new bool IsNull { get; }

        bool ITerraformValue.IsNull => IsNull;

        /// <summary>
        /// A shortcut for <see cref="TerraformValue{T}.Null"/>.
        /// </summary>
        new ITerraformValue<T> AsNull => TerraformValue<T>.Null;

        /// <summary>
        /// <see langword="true"/> indicates, that <see cref="Value"/> is not yet known. <see langword="true"/> also indicates, that <see cref="IsNull"/> is implicitly <see langword="true"/> too.
        /// </summary>
        new bool IsUnknown { get; }

        bool ITerraformValue.IsUnknown => IsUnknown;

        /// <summary>
        /// A shortcut for <see cref="TerraformValue{T}.Unknown"/>.
        /// </summary>
        new ITerraformValue<T> AsUnknown => TerraformValue<T>.Unknown;

        void ITerraformValue.Encode<TEncoder>(TEncoder encoder, ref MessagePackWriter writer, ValueDefinition value) =>
            encoder.Encode(ref writer, value, this);
    }
}
