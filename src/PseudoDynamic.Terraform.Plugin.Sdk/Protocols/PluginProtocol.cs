namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    /// <summary>
    /// Specifies the plugin protocol to be used by the gRPC server.
    /// </summary>
    public enum PluginProtocol
    {
        /// <summary>
        /// Intends to start the plugin server with the protocol version 5.
        /// </summary>
        V5,
        /// <summary>
        /// Intends to start the plugin server with the protocol version 5.
        /// </summary>
        V6
    }
}
