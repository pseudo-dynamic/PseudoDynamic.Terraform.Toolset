using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Features
{
    public abstract class ProviderFeaturesBase<ProviderMetaSchema> : ProviderMetaSupportBase<ProviderMetaSchema>, IProviderFeature
    {
        public IServiceCollection Services { get; internal init; }

        internal override IProviderFeature ProviderFeature => this;

        internal ProviderFeaturesBase(IServiceCollection services) =>
            Services = services ?? throw new ArgumentNullException(nameof(services));
    }
}
