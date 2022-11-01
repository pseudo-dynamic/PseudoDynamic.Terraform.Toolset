using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Allows to modify the Terraform provider.
    /// </summary>
    public interface IProviderFeature
    {
        /// <summary>
        /// The original service collection.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
