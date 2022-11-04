using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    internal static class TerraformDynamicMessagePackEncoderExtensions
    {
        public static ReadOnlyMemory<byte> EncodeBlock<T>(this TerraformDynamicMessagePackEncoder encoder, BlockDefinition block, [DisallowNull] T content)
        {
            if (content is null) {
                throw new TerraformDynamicMessagePackEncodingException("You cannot encode an object with the schema of a block that is null, because blocks can never be null");
            }

            return encoder.EncodeValue(block, content);
        }
    }
}
