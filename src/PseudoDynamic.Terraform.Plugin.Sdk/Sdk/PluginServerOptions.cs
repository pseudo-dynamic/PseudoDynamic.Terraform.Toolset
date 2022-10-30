using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting.Server.Features;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Contains the general Terraform plugin options.
    /// </summary>
    public class PluginServerOptions
    {
        /// <summary>
        /// The intended plugin protocol version to be used by the gRPC server.
        /// </summary>
        public PluginProtocol? Protocol { get; internal set; }

        /// <summary>
        /// By default, HTTPS is enabled, which is required by Terraform in production environment.
        /// However, if this value is set to <see langword="true"/>, HTTP is used instead and the
        /// instructions for Terraform using this debug instance are displayed in the console output.
        /// </summary>
        public bool IsDebuggable { get; set; }

        /// <summary>
        /// Required, if you setup Kestrel by your own and use more than one server address.
        /// The default behaviour is to select the first server addresss provided by
        /// <see cref="IServerAddressesFeature.Addresses"/>. By letting the filter returning
        /// <see langword="null"/> you can probe for the next server address, if existing,
        /// but you must select a address, otherwise an exception is thrown.
        /// </summary>
        /// <remarks>
        /// Ensure returning a server address that contains an valid ip address and not a
        /// domain name, otherwise Terraform may have trouble to contact this server.
        /// </remarks>
        public Func<string, string?>? ServerAddressFilter { get; set; }

        /// <summary>
        /// The client certificate Terraform will use to contact this server. If setting it, you
        /// should customize Kestrel in such a way, that it accepts this client certificate.
        /// </summary>
        /// <remarks>
        /// Do not set it, if you are using <see cref="PluginServerWebHostBuilderExtensions.UseTerraformPluginServer{THostBuilder}(THostBuilder, IPluginServerSpecification.ProtocolV5)"/>
        /// or similiar, otherwsie an exception is thrown. This is because this method sets up a default
        /// listener for Kestrel with HTTPS enabled and a self-signed certificate.
        /// </remarks>
        public X509Certificate2? ClientCertificate { get; set; }
    }
}
