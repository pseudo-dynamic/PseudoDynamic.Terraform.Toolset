namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    /// <summary>
    /// When using this, it allows you to decode unknown schema parts, that are of type <see langword="object"/>.
    /// </summary>
    public interface ITerraformDynamicDecoder
    {
        /// <summary>
        /// Tries to decodes an unknown part of your schema.
        /// </summary>
        /// <typeparam name="Schema">The type you want to try to decode this unknown schema part to.</typeparam>
        /// <param name="unknown"></param>
        /// <param name="content">The resulting content.</param>
        /// <returns><see langword="true"/> if the decoding was successful.</returns>
        bool TryDecode<Schema>(object? unknown, [NotNullWhen(true)] out Schema? content);
    }
}
