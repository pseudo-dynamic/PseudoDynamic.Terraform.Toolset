using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderBuilder : IProviderBuilder
    {
        public IServiceCollection Services { get; }

        public ProviderBuilder(IServiceCollection services) =>
            Services = services;
    }
}
