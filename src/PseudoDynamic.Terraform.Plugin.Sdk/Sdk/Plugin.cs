using Grpc.Core;
using PseudoDynamic.Terraform.Plugin.Protocol_V6;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class PluginServer : IDisposable
    {
        public string Address => $"https://{_serverPort.Host}:{_serverPort.BoundPort}";

        private Server _server;
        private ServerPort _serverPort;

        public PluginServer()
        {
            var provider = new ProviderAdapter();
            _serverPort = new ServerPort("127.0.0.1", 0, ServerCredentials.Insecure);

            // ISSUE: ServerCredentials: enable SSL: generate certificate
            _server = new Server {
                Services = {
                   Protocol_V6.Provider.BindService(provider),
                },
                Ports = { new ServerPort("127.0.0.1", 0, ServerCredentials.Insecure) }
            };
        }

        public void Start()
        {
            _server.Start();
        }

        public void Dispose() =>
            _server.ShutdownAsync();
    }
}
