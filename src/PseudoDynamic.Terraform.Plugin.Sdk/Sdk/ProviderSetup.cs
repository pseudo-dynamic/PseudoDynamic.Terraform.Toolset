using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderSetup : IProviderSetup
    {
        public IServiceCollection Services { get; }

        public ProviderSetup(IServiceCollection services) =>
            Services = services;
    }
}
