using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Features
{
    public abstract class ProviderFeaturesBase : ProviderMetaFeaturesBase<object>, IProviderFeature
    {
        public IServiceCollection Services { get; }

        internal override IProviderFeature ProviderFeature => this;

        internal ProviderFeaturesBase(IServiceCollection services) =>
            Services = services ?? throw new ArgumentNullException(nameof(services));

        public void UseProviderMeta<ProviderMetaSchema>(Action<ProviderMetaFeatures<ProviderMetaSchema>> configure)
            where ProviderMetaSchema : class
        {
            if (configure is null) {
                throw new ArgumentNullException(nameof(configure));
            }

            ProviderFeature.SetProviderMeta(ProviderMetaFeatures<ProviderMetaSchema>.ProviderMetaSchemaType);
            configure(new ProviderMetaFeatures<ProviderMetaSchema>(this));
        }
    }
}
