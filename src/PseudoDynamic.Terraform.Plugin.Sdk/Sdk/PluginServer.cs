using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class PluginServer : IPluginServer
    {
        internal const PluginProtocol DefaultProtocol = PluginProtocol.V6;

        public Uri ServerAddress => _serverAddress.Value;
        public PluginProtocol PluginProtocol { get; }
        public bool IsDebuggable { get; }
        public CancellationToken ServerStarted { get; }
        public CancellationToken ServerStopping { get; }
        public CancellationToken ServerStopped { get; }

        private readonly X509Certificate2? _clientCertificate;
        private readonly Lazy<Uri> _serverAddress;
        private readonly ILogger<PluginServer> _logger;

        public PluginServer(
            IServer server,
            IOptions<PluginServerOptions> options,
            IHostApplicationLifetime applicationLifetime,
            ILogger<PluginServer> logger)
        {
            var unwrappedOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            PluginProtocol = unwrappedOptions.Protocol ?? throw new InvalidOperationException($"The plugin protocol has not been configured inside an instance of {typeof(PluginServerOptions).FullName}");
            IsDebuggable = unwrappedOptions.IsDebuggable;

            if (!IsDebuggable) {
                _clientCertificate = unwrappedOptions.ClientCertificate ?? throw new InvalidOperationException($"The plugin client certificate has not been configured inside an instance of {typeof(PluginServerOptions).FullName}");
            }

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
                    var serverAddressBuilder = new UriBuilder(serverAddressUri) {
                        Host = "127.0.0.1"
                    };

                    return serverAddressBuilder.Uri;
                } else {
                    return serverAddressUri;
                }
            });

            applicationLifetime.ApplicationStarted.Register(OnServerStarted);
            ServerStarted = applicationLifetime.ApplicationStarted;

            applicationLifetime.ApplicationStopping.Register(OnServerStopping);
            ServerStopping = applicationLifetime.ApplicationStopping;

            applicationLifetime.ApplicationStopped.Register(OnServerStopped);
            ServerStopped = applicationLifetime.ApplicationStopped;

            _logger = logger;
        }

        private void WriteTerraformHandshake()
        {
            var serverAddress = ServerAddress;
            // Terraform seems not to like Base64 padding, so we trim
            var base64EncodedCertificate = Convert.ToBase64String(_clientCertificate!.RawData).TrimEnd('=');
            var terraformHandshake = $"1|{PluginProtocol.ToVersionNumber()}|tcp|{serverAddress.Host}:{serverAddress.Port}|grpc|{base64EncodedCertificate}";
            _logger.LogInformation($"Writing Terraform handshake{Environment.NewLine}" + terraformHandshake);
            // Terraform reads until newline and stucks if non is present
            Console.WriteLine(terraformHandshake);
        }

        private void OnServerStarted()
        {
            _logger.LogInformation("Started plugin server");

            if (!IsDebuggable) {
                WriteTerraformHandshake();
            }
        }

        private void OnServerStopping() => _logger.LogInformation("Stopping plugin server");

        private void OnServerStopped() => _logger.LogInformation("Stopped plugin server");
    }
}
