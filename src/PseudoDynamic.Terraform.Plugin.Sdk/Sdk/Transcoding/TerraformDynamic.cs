namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
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
        /// The bytes that represents the unknown part of the schema.
        /// </summary>
        public ReadOnlyMemory<byte> Memory { get; }

        public TerraformDynamicEncoding Encoding { get; }

        public TerraformDynamic(ReadOnlyMemory<byte> memory, TerraformDynamicEncoding encoding)
        {
            Memory = memory;
            Encoding = encoding;
        }
    }
}
