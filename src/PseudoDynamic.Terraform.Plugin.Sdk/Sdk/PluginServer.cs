using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class PluginServer : IPluginServer
    {
        public Uri ServerAddress => _serverAddress;

        Uri _serverAddress;

        public PluginServer(IServer server)
        {
            var serverAddresses = server.Features.Get<IServerAddressesFeature>();

            if (serverAddresses is null) {
                throw new InvalidOperationException("The plugin server needs at least one available server address");
            }

            var serverAddressString = serverAddresses.Addresses.First();
            var serverAddressUri = new Uri(serverAddressString);

            if (string.Equals(serverAddressUri.Host, "localhost", StringComparison.InvariantCultureIgnoreCase)) {
                var serverAddressBuilder = new UriBuilder(serverAddressUri);
                serverAddressBuilder.Host = "127.0.0.1";
                _serverAddress = serverAddressBuilder.Uri;
            } else {
                _serverAddress = serverAddressUri;
            }
        }
    }
}
