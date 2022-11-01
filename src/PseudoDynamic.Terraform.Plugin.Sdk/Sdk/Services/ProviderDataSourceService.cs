namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class ProviderDataSourceService : DataSourceService, INameProvider
    {
        public string FullTypeName { get; }
        string INameProvider.Name => FullTypeName;

        public ProviderDataSourceService(DataSourceService dataSource, string fullTypeName)
            : base(dataSource) =>
            FullTypeName = fullTypeName;
    }
}
