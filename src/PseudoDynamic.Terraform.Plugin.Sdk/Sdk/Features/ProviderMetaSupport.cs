namespace PseudoDynamic.Terraform.Plugin.Sdk.Features
{
    public sealed class ProviderMetaSupport<ProviderMetaSchema> : ProviderMetaSupportBase<ProviderMetaSchema>
        where ProviderMetaSchema : class
    {
        internal override IProviderFeature ProviderFeature { get; }

        internal ProviderMetaSupport(IProviderFeature provider) =>
            ProviderFeature = provider ?? throw new ArgumentNullException(nameof(provider));
    }
}
