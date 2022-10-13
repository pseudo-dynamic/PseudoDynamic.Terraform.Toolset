using System.Text.Json.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class TerraformReattachProvider
    {
        public string Protocol { get; init; } = "grpc";

        [JsonPropertyName("Pid")]
        public int ProcessId { get; init; } = Environment.ProcessId;

        [JsonPropertyName("Test")]
        public bool IsTest { get; init; } = true;

        [JsonPropertyName("Addr")]
        public TerraformReattachProviderAddress Address { get; }

        public TerraformReattachProvider(TerraformReattachProviderAddress address) =>
            Address = address ?? throw new ArgumentNullException(nameof(address));
    }
}