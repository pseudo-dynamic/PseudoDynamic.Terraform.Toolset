namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IProvider<Schema> : IProvider where Schema : class
    {
        internal new static readonly ProviderAdapter.ProviderGenericAdapter<Schema> ProviderAdapter;

        ProviderAdapter.IProviderAdapter IProvider.ProviderAdapter => ProviderAdapter;

        Task Configure(Provider.ConfigureContext<Schema> context);
    }
}
