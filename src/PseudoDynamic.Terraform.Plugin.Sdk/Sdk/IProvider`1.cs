namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IProvider<Schema> : IProvider where Schema : class
    {
        internal static new readonly ProviderAdapter.ProviderGenericAdapter<Schema> ProviderAdapter;

        ProviderAdapter.IProviderAdapter IProvider.ProviderAdapter => ProviderAdapter;

        Task ValidateConfig(Provider.IValidateConfigContext<Schema> context);

        Task Configure(Provider.IConfigureContext<Schema> context);
    }
}
