using MessagePack;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Represents the low-level representation of <see cref="ITerraformValue{T}"/>.
    /// If it is used as part of a schema, then <see cref="ITerraformValue"/> will implicitly
    /// represent <see cref="ITerraformValue{T}"/> with T of type <see cref="object"/> which
    /// in turn will <see cref="Value"/> let result into a Terraform dynamic value, that needs
    /// first to be decoded.
    /// </summary>
    public interface ITerraformValue
    {
        /// <summary>
        /// If <see cref="Value"/> gets not overriden, then it represents a Terraform
        /// dynamic value, that needs first to be decoded.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// <see langword="true"/> means <see cref="Value"/> is <see langword="null"/>.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        bool IsNull { get; }

        /// <summary>
        /// A shortcut for <see cref="TerraformValue{T}.Null"/>.
        /// </summary>
        ITerraformValue<object> AsNull => TerraformValue<object>.Null;

        /// <summary>
        /// <see langword="true"/> indicates, that <see cref="Value"/> is not yet known. <see langword="true"/> also indicates, that <see cref="IsNull"/> is implicitly <see langword="true"/> too.
        /// </summary>
        bool IsUnknown { get; }

        /// <summary>
        /// A shortcut for <see cref="TerraformValue{T}.Unknown"/>.
        /// </summary>
        ITerraformValue<object> AsUnknown => TerraformValue<object>.Unknown;

        internal void Encode<TEncoder>(TEncoder encoder, ref MessagePackWriter writer, ValueDefinition value)
            where TEncoder : ITerraformValueMessagePackEncoder =>
            encoder.Encode(ref writer, value, this);
    }
}
