using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Contains the general Terraform plugin options.
    /// </summary>
    public class PluginOptions
    {
        /// <summary>
        /// The intended protocol version to be used for the gRPC server.
        /// </summary>
        public PluginProtocol? Protocol { get; set; }
    }
}
