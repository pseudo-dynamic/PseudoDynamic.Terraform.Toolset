namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Used as placeholder for <see langword="object"/>'s in decoding schemas. Allows to distinguish
    /// between not yet and already decoded dynamic values in case the user replaces the unknown part
    /// of the schema with another object that is not <see cref="TerraformDynamic"/>. Used mainly for
    /// <see cref="ITerraformDynamicDecoder"/>.
    /// </summary>
    internal sealed class TerraformDynamic
    {
        /// <summary>
        /// The MessagePack bytes that represents the unknown part of the schema.
        /// </summary>
        public ReadOnlyMemory<byte> MessagePackBytes { get; }

        public TerraformDynamic(ReadOnlyMemory<byte> messagePackBytes) =>
            MessagePackBytes = messagePackBytes;
    }
}
