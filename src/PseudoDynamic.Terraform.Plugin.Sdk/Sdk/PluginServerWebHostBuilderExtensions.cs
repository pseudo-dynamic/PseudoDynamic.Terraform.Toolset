using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk.Kestrel;
using Serilog;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class PluginServerWebHostBuilderExtensions
    {
        internal static THostBuilder UseTerraformPluginServerCore<THostBuilder>(
            this THostBuilder builder,
            IPluginServerSpecification serverSpecification)
            where THostBuilder : IWebHostBuilder
        {
            if (!serverSpecification.IsDebuggable) {
                // Necessary to prevent writing to console due to Terraform handshake
                builder.UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "true");
            }

            builder
                .UseKestrel()
                .ConfigureLogging(logging => {
                    if (!serverSpecification.IsDebuggable) {
                        logging.ClearProviders();

#if DEBUG
                        var logger = new LoggerConfiguration()
                            .WriteTo.File("logs.txt")
                            .CreateLogger();

                        logging.AddSerilog(logger, dispose: true);
#endif
                    }
                })
                .ConfigureServices(services => {
                    services.AddKestrelLoopbackListener();
                    services.AddTerraformPluginServer();
                })
                .Configure(app => {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapTerraformPlugin());
                    app.UseTerraformPluginShutdown();
                });

            return builder;
        }

        public static THostBuilder UseTerraformPluginServer<THostBuilder>(
            this THostBuilder builder,
            IPluginServerSpecification.ProtocolV5 serverSpecification)
            where THostBuilder : IWebHostBuilder
        {
            builder
                .UseTerraformPluginServerCore(serverSpecification)
                .ConfigureServices(services => services.AddTerraformPluginServer(serverSpecification));

            return builder;
        }

        public static THostBuilder UseTerraformPluginServer<THostBuilder>(
            this THostBuilder builder,
            IPluginServerSpecification.ProtocolV6 serverSpecification)
            where THostBuilder : IWebHostBuilder
        {
            builder
                .UseTerraformPluginServerCore(serverSpecification)
                .ConfigureServices(services => services.AddTerraformPluginServer(serverSpecification));

            return builder;
        }
    }
}
