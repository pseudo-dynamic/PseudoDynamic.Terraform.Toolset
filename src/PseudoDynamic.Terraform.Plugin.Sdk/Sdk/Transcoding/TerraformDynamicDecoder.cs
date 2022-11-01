using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    internal class TerraformDynamicDecoder : ITerraformDynamicDecoder
    {
        private readonly TerraformDynamicMessagePackDecoder _decoder;

        public TerraformDynamicDecoder(TerraformDynamicMessagePackDecoder decoder) =>
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));

        public bool TryDecode<Schema>(object? unknown, [NotNullWhen(true)] out Schema? content)
        {
            try {
                var reports = new Reports();
                content = (Schema)_decoder.DecodeDynamic(unknown, typeof(Schema), new TerraformDynamicMessagePackDecoder.DecodingOptions())!;
                return true;
            } catch {
                content = default;
                return false;
            }
        }
    }
}
