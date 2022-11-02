using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    public abstract class PluginHostFixtureBase : IAsyncLifetime, IPluginServerSpecification
    {
        public virtual string ProviderName => "registry.terraform.io/pseudo-dynamic/debug";
        public abstract PluginProtocol Protocol { get; }
        public bool IsDebuggable => true;
        public IWebHost Host { get; private set; } = null!;

        internal IProviderContext Provider { get; private set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        internal TerraformCommand.WorkingDirectoryCloning CreateTerraformCommand(Action<TerraformCommand.WorkingDirectoryCloning.WorkingDirectoryCloningOptions>? configureOptions = null) =>
            Host.Services.GetWorkingDirectoryCloningTerraformCommand(configureOptions);

        /// <summary>
        /// Creates the Terraform command and calls terraform init.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        internal TerraformCommand.WorkingDirectoryCloning CreateTerraformCommand(string workingDirectory) =>
            CreateTerraformCommand(options => options.WorkingDirectory = workingDirectory);

        /// <summary>
        /// Creates the Terraform command and calls terraform init.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="copyableFilePatterns">
        /// Specifies custom file patterns that are used to copy matching files from working directory to the temporary directory.
        /// </param>
        /// <returns></returns>
        internal TerraformCommand.WorkingDirectoryCloning CreateTerraformCommand(string workingDirectory, params string[] copyableFilePatterns) => CreateTerraformCommand(options =>
        {
            options.WorkingDirectory = workingDirectory;
            options.CopyableFilePatterns = copyableFilePatterns;
        });

        public async Task InitializeAsync()
        {
            var host = new WebHostBuilder()
                .UseTerraformPluginServerCore(this)
                .ConfigureServices(services =>
                {
                    services.AddOptions<ProviderOptions>().Configure(options =>
                    {
                        options.FullyQualifiedProviderName = ProviderName;
                    });

                    services.AddOptions<PluginServerOptions>().Configure(options =>
                    {
                        options.Protocol = Protocol;
                        options.IsDebuggable = true;
                    });

                    services.AddTerraformProvider();
                })
                .Build();

            Host = host;
            await host.StartAsync();
            Provider = host.Services.GetRequiredService<IProviderContext>();
        }

        public Task DisposeAsync() => Host.StopAsync();
    }
}
