namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal class ProviderDataSourceServiceRegistry : NamedTerraformServiceRegistry<ProviderDataSourceService>
    {
        private readonly DataSourceServiceFactory _dataSourceDefinitionFactory;
        private readonly IProviderServer _providerServer;

        public ProviderDataSourceServiceRegistry(DataSourceServiceFactory dataSourceDefinitionFactory, IProviderServer providerServer)
        {
            _dataSourceDefinitionFactory = dataSourceDefinitionFactory ?? throw new ArgumentNullException(nameof(dataSourceDefinitionFactory));
            _providerServer = providerServer ?? throw new ArgumentNullException(nameof(providerServer));
        }

        private ProviderDataSourceService UpgradeDataSource(DataSourceServiceDescriptor dataSourceDescriptor)
        {
            DataSourceService dataSource = _dataSourceDefinitionFactory.Build(dataSourceDescriptor);
            string dataSourceName = dataSource.Implementation.Name;
            string fullDataSourceName = $"{_providerServer.SnakeCaseProviderName}_{dataSourceName}";
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
