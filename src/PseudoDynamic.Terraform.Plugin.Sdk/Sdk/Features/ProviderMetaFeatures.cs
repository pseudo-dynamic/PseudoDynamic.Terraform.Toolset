namespace PseudoDynamic.Terraform.Plugin.Sdk.Features
{
    public sealed class ProviderMetaFeatures<ProviderMetaSchema> : ProviderMetaFeaturesBase<ProviderMetaSchema>
        where ProviderMetaSchema : class
    {
        internal override IProviderFeature ProviderFeature { get; }

        internal ProviderMetaFeatures(IProviderFeature provider) =>
            ProviderFeature = provider ?? throw new ArgumentNullException(nameof(provider));
    }
}
