namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Represents the plugin server.
    /// </summary>
    public interface IPluginServer
    {
        Uri ServerAddress { get; }
    }
}
