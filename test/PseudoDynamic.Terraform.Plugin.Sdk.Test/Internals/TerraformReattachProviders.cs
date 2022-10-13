using System.Text.Json;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class TerraformReattachProviders : Dictionary<string, TerraformReattachProvider>
    {
        public const string VariableName = "TF_REATTACH_PROVIDERS";

        internal string ToJson() => JsonSerializer.Serialize(this);
    }
}
