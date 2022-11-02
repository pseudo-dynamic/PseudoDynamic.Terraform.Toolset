using System.Text.Json.Serialization;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class TerraformReattachProvider
    {
        public string Protocol { get; init; } = "grpc";
        public int ProtocolVersion { get; private set; }

        [JsonPropertyName("Pid")]
        public int ProcessId { get; init; } = Environment.ProcessId;

        [JsonPropertyName("Test")]
        public bool IsTest { get; init; } = true;

        [JsonPropertyName("Addr")]
        public TerraformReattachProviderAddress Address { get; }

        public TerraformReattachProvider(PluginProtocol protocol, TerraformReattachProviderAddress address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            SetProtocolVersion(protocol);
        }

        private void SetProtocolVersion(PluginProtocol protocol)
        {
            if (protocol == PluginProtocol.V5) {
                ProtocolVersion = 5;
            } else if (protocol == PluginProtocol.V6) {
                ProtocolVersion = 6;
            } else {
                throw new NotSupportedException("Bad protocol");
            }
        }
    }
}
