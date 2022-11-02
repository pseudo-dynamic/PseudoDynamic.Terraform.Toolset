namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal class ProviderDataSourceServiceRegistry : NamedTerraformServiceRegistry<ProviderDataSourceService>
    {
        DataSourceServiceFactory _dataSourceDefinitionFactory;
        private readonly IProviderServer _providerServer;

        public ProviderDataSourceServiceRegistry(DataSourceServiceFactory dataSourceDefinitionFactory, IProviderServer providerServer)
        {
            _dataSourceDefinitionFactory = dataSourceDefinitionFactory ?? throw new ArgumentNullException(nameof(dataSourceDefinitionFactory));
            _providerServer = providerServer ?? throw new ArgumentNullException(nameof(providerServer));
        }

        private ProviderDataSourceService UpgradeDataSource(DataSourceServiceDescriptor dataSourceDescriptor)
        {
            var dataSource = _dataSourceDefinitionFactory.Build(dataSourceDescriptor);
            var dataSourceName = dataSource.Implementation.Name;
            var fullDataSourceName = $"{_providerServer.SnakeCaseProviderName}_{dataSourceName}";
            return new ProviderDataSourceService(dataSource, fullDataSourceName);
        }

        public void Add(DataSourceServiceDescriptor dataSourceDescriptor) =>
            Add(UpgradeDataSource(dataSourceDescriptor));

        /// <summary>
        /// Adds or replaces a dataSource described by <paramref name="dataSourceDescriptor"/>.
        /// </summary>
        /// <param name="dataSourceDescriptor"></param>
        internal void Replace(DataSourceServiceDescriptor dataSourceDescriptor) =>
            Replace(UpgradeDataSource(dataSourceDescriptor));
    }
}
