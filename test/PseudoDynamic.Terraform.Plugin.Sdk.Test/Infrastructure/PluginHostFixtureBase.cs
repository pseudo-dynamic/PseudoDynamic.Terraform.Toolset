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
        /// <param name="workingDirectory"></param>
        /// <param name="preventInit">Prevents from calling terraform init to initialize the working directory.</param>
        /// <returns></returns>
        internal TerraformCommand CreateTerraformCommand(string workingDirectory, bool preventInit = false)
        {
            var terraform = Host.Services.GetWorkingDirectoryCloningTerraformCommand(workingDirectory);

            if (!preventInit)
            {
                Record.Exception(terraform.Init).Should().BeNull();
            }

            return terraform;
        }

        public async Task InitializeAsync()
        {
            var host = new PluginHostBuilder() { Protocol = PluginProtocol }
                .ConfigureTerraformProviderDefaults(ProviderName)
                .Build();

            Host = host;
            await host.StartAsync();
            Provider = host.Services.GetRequiredService<IProvider>();
        }

        public Task DisposeAsync() => Host.StopAsync();
    }
}
