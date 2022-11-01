namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IProvider
    {
        internal ProviderAdapter.IProviderAdapter ProviderAdapter { get; }
    }
}
