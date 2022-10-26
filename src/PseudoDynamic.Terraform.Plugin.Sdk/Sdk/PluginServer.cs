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

        private Lazy<Uri> _serverAddress;

        public PluginServer(IServer server, IOptions<PluginServerOptions> options)
        {
            var unwrappedOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            PluginProtocol = unwrappedOptions.Protocol ?? throw new InvalidOperationException($"The plugin protocol has not been configured inside an instance of {typeof(PluginServerOptions).FullName}");

            // Server port is only initialized after the server has been started, therefore we delay it because the
            // access to the lazy server address of this instance is expected to happen after the server started.
            _serverAddress = new Lazy<Uri>(() => {
                var serverAddressesProvider = server.Features.Get<IServerAddressesFeature>();

                if (serverAddressesProvider is null || serverAddressesProvider.Addresses.Count == 0) {
                    throw new InvalidOperationException("The plugin server needs at least one available server address");
                }

                string? serverAddressString;

                if (unwrappedOptions.ServerAddressFilter != null) {
                    var enumerator = serverAddressesProvider.Addresses.GetEnumerator();

                    do {
                        if (!enumerator.MoveNext()) {
                            throw new InvalidOperationException("The custom server address filter could estimate a server address");
                        }

                        if (enumerator.Current != null) {
                            serverAddressString = enumerator.Current;
                            break;
                        }
                    } while (true);
                } else {
                    serverAddressString = serverAddressesProvider.Addresses.First();
                }

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
