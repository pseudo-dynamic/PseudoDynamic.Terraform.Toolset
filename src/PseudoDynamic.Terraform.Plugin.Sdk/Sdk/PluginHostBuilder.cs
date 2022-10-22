using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class PluginHostBuilder : HostBuilder, IPluginHostBuilder
    {
        public PluginProtocol Protocol { get; init; } = PluginProtocol.V6;

        public new IHost Build()
        {
            ConfigureServices((_, services) => services.AddOptions<PluginOptions>().Configure(o => o.Protocol = Protocol));
            return base.Build();
        }

        IHost IHostBuilder.Build() => Build();
    }
}
