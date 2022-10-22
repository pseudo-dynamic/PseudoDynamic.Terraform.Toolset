using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class PluginServer : IPluginServer
    {
        public Uri ServerAddress => _serverAddress.Value;
        public PluginProtocol PluginProtocol { get; }

        Lazy<Uri> _serverAddress;

        public PluginServer(IServer server, IOptions<PluginOptions> pluginOptions)
        {
            var unwrappedPluginOptions = pluginOptions?.Value ?? throw new ArgumentNullException(nameof(pluginOptions));
            PluginProtocol = unwrappedPluginOptions.Protocol ?? throw new InvalidOperationException($"The plugin protocol has not been configured inside an instance of {typeof(PluginOptions).FullName}");

            // Server port is only initialized after the server has been started, therefore we delay it because the
            // access to the lazy server address of this instance is expected to happen after the server started.
            _serverAddress = new Lazy<Uri>(() => {
                var serverAddresses = server.Features.Get<IServerAddressesFeature>();

                if (serverAddresses is null) {
                    throw new InvalidOperationException("The plugin server needs at least one available server address");
                }

                var serverAddressString = serverAddresses.Addresses.First();
                var serverAddressUri = new Uri(serverAddressString);

                if (serverAddressUri.Port == 0) {
                    throw new InvalidOperationException($"Because the server port of the address {serverAddressString} is still zero, you cannot access the server address. Has the server been started?");
                }

                if (string.Equals(serverAddressUri.Host, "localhost", StringComparison.InvariantCultureIgnoreCase)) {
                    var serverAddressBuilder = new UriBuilder(serverAddressUri);
                    serverAddressBuilder.Host = "127.0.0.1";
                    return serverAddressBuilder.Uri;
                } else {
                    return serverAddressUri;
                }
            });
        }
    }
}
