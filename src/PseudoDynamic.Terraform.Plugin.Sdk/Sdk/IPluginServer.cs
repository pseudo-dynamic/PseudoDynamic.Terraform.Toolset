using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Represents the plugin server.
    /// </summary>
    public interface IPluginServer
    {
        Uri ServerAddress { get; }
        PluginProtocol PluginProtocol { get; }
        bool IsDebuggable { get; }
        CancellationToken ServerStarted { get; }
        CancellationToken ServerStopping { get; }
        CancellationToken ServerStopped { get; }
    }
}
