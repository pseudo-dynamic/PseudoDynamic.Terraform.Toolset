using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Kestrel
{
    internal class KestrelLoopbackListenerConfiguration : IPostConfigureOptions<KestrelServerOptions>
    {
        private PluginServerOptions _pluginServerOptions;

        public KestrelLoopbackListenerConfiguration(IOptions<PluginServerOptions> pluginServerOptions) =>
            _pluginServerOptions = pluginServerOptions?.Value ?? throw new ArgumentNullException(nameof(pluginServerOptions));

        public void PostConfigure(string _, KestrelServerOptions options)
        {
            if (_pluginServerOptions.IsDebuggable) {
                options.Listen(IPAddress.Loopback, port: 0, options => options.Protocols = HttpProtocols.Http2);
            } else {
                throw new NotImplementedException();
                // TODO: generate certificate
                //options.ListenAnyIP(0, options => options.UseHttps(..);
            }
        }
    }
}
