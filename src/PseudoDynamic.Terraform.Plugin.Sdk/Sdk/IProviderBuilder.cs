using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IProviderBuilder
    {
        IServiceCollection Services { get; }
    }
}
