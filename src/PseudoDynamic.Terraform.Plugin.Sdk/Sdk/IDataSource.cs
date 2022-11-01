namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IDataSource : INameProvider
    {
        internal ProviderAdapter.IDataSourceAdapter DataSourceAdapter { get; }
    }
}
