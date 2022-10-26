using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    public abstract class PluginHostFixtureBase : IAsyncLifetime
    {
        public virtual string ProviderName => "registry.terraform.io/pseudo-dynamic/debug";

        public abstract PluginProtocol PluginProtocol { get; }

        public IHost Host { get; private set; } = null!;
        internal IProvider Provider { get; private set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureOptions"></param>
        /// <param name="preventInit">Prevents from calling terraform init to initialize the working directory.</param>
        /// <returns></returns>
        internal TerraformCommand.WorkingDirectoryCloning CreateTerraformCommand(Action<TerraformCommand.WorkingDirectoryCloning.WorkingDirectoryCloningOptions>? configureOptions = null, bool preventInit = false)
        {
            var terraform = Host.Services.GetWorkingDirectoryCloningTerraformCommand(configureOptions);

            if (!preventInit)
            {
                Record.Exception(terraform.Init).Should().BeNull();
            }

            return terraform;
        }

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
            var host = new PluginHostBuilder() { Protocol = PluginProtocol, IsDebuggable = true, }
                .ConfigureTerraformProviderDefaults(ProviderName)
                .Build();

            Host = host;
            await host.StartAsync();
            Provider = host.Services.GetRequiredService<IProvider>();
        }

        public Task DisposeAsync() => Host.StopAsync();
    }
}
