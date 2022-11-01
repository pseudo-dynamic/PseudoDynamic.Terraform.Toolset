namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class ProviderResourceService : ResourceService, INameProvider
    {
        public string FullTypeName { get; }
        string INameProvider.Name => FullTypeName;

        public ProviderResourceService(ResourceService resource, string fullTypeName)
            : base(resource) =>
            FullTypeName = fullTypeName;
    }
}
