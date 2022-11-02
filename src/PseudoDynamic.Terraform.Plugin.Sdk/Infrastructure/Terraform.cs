using System.Text.Json;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class Terraform
    {
        internal const string TfReattachProvidersVariableName = "TF_REATTACH_PROVIDERS";
        internal const string TfCliConfigFileVariableName = "TF_CLI_CONFIG_FILE";
        internal const string TerraformCommandName = "terraform";

        public static string SerializeTerraformReattachProviders(IReadOnlyDictionary<string, TerraformReattachProvider>? terraformReattachProviders) =>
            JsonSerializer.Serialize(terraformReattachProviders);
    }
}
