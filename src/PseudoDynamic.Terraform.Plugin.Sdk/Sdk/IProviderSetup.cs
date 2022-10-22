using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Allows you to modify the Terraform provider.
    /// </summary>
    public interface IProviderSetup
    {
        /// <summary>
        /// The original service collection.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
