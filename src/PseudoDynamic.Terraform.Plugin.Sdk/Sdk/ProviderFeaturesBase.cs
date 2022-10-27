using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class ProviderFeaturesBase : IProviderFeature, IProviderFeature.IResourceFeature, IProviderFeature.IDataSourceFeature
    {
        public IServiceCollection Services { get; }

        IProviderFeature IProviderFeature.IResourceFeature.ProviderFeature => this;

        IProviderFeature IProviderFeature.IDataSourceFeature.ProviderFeature => this;

        internal ProviderFeaturesBase(IServiceCollection services) =>
            Services = services ?? throw new ArgumentNullException(nameof(services));
    }
}
