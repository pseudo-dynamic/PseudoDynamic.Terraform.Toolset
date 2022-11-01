namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IResource : INameProvider
    {
        internal ProviderAdapter.IResourceAdapter ResourceAdapter { get; }
    }
}
