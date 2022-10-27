namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal static class PluginProtocolExtensions
    {
        public static int ToVersionNumber(this PluginProtocol protocol) => protocol switch {
            PluginProtocol.V5 => 5,
            PluginProtocol.V6 => 6,
            _ => throw new NotSupportedException($"The plugin protocol version {protocol} is not supported")
        };
    }
}
