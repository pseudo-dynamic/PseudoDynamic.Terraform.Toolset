using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Protocols.V5;
using PseudoDynamic.Terraform.Plugin.Protocols.V6;
using PseudoDynamic.Terraform.Plugin.Sdk;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal static class PluginServerExtensions
    {
        /// <summary>
        /// Adds a middleware that listens to the hashicorp/go-plugin's "plugin.GRPCController/Shutdown" gRPC call.
        /// Calls <see cref="IHostApplicationLifetime.StopApplication"/> to increase shutdown time dramatically.
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseTerraformPluginShutdown(this IApplicationBuilder app)
        {
            app.Use(async (context, next) => {
                await next();

                if (context.Request.Method == HttpMethod.Post.Method
                    && context.Request.ContentType == "application/grpc"
                    && context.Request.Path == "/plugin.GRPCController/Shutdown") {
                    IHostApplicationLifetime server = context.RequestServices.GetRequiredService<IHostApplicationLifetime>();
                    server.StopApplication();
                }
            });

            return app;
        }

        /// <summary>
        /// Tells gRPC what protocol-specific implementation needs to be called.
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="NotSupportedException"></exception>
        public static IEndpointRouteBuilder MapTerraformPlugin(this IEndpointRouteBuilder builder)
        {
            IPluginServer pluginServer = builder.ServiceProvider.GetRequiredService<IPluginServer>();
            PluginProtocol pluginProtocol = pluginServer.PluginProtocol;

            if (pluginProtocol == PluginProtocol.V5) {
                builder.MapTerraformPluginProtocolV5();
            } else if (pluginProtocol == PluginProtocol.V6) {
                builder.MapTerraformPluginProtocolV6();
            } else {
                throw new NotSupportedException("Bad protocol");
            }

            return builder;
        }
    }
}
