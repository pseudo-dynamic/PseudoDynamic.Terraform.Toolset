using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Allows you to modify the Terraform provider.
    /// </summary>
    public interface IProviderFeature
    {
        /// <summary>
        /// The original service collection.
        /// </summary>
        IServiceCollection Services { get; }

        public interface IResourceFeature
        {
            internal IProviderFeature ProviderFeature { get; }
        }

        internal readonly struct ResourceFeature : IResourceFeature
        {
            internal IProviderFeature ProviderFeature { get; }

            IProviderFeature IResourceFeature.ProviderFeature => ProviderFeature;

            internal ResourceFeature(IProviderFeature providerFeature) =>
                ProviderFeature = providerFeature;
        }

        public interface IDataSourceFeature
        {
            internal IProviderFeature ProviderFeature { get; }
        }

        public interface IProvisionerFeature
        {
            internal IProviderFeature ProviderFeature { get; }
        }
    }
}
