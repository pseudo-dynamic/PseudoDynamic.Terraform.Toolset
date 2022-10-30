namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal interface IProviderContext
    {
        string FullyQualifiedProviderName { get; }
        string ProviderName { get; }
        string SnakeCaseProviderName { get; }

        ProviderService? ProviderService { get; }
        IReadOnlyDictionary<string, ProviderResourceService> ResourceServices { get; }
        IReadOnlyDictionary<string, ProviderDataSourceService> DataSourceServices { get; }

        internal void ReplaceResource(ResourceServiceDescriptor resourceDescriptor);
    }
}
