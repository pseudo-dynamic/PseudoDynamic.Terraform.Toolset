using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Schema.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// <para>
    /// Represents the adapter for <see cref="DynamicValueEncoder"/>.
    /// </para>
    /// <para>
    /// Not intended being consumed by third-parties!
    /// </para>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ITerraformValue
    {
        /// <summary>
        /// The deserialized Terraform value.
        /// </summary>
        internal object? Value { get; }

        /// <summary>
        /// True means <see cref="Value"/> is null.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Value))]
        internal bool IsNull { get; }

        /// <summary>
        /// True indicates, that <see cref="Value"/> is not yet known.
        /// </summary>
        internal bool IsUnknown { get; }
    }
}
