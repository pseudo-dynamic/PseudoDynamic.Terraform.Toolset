namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderDataSourceServiceRegistry : NamedTerraformServiceRegistry<ProviderDataSourceService>
    {
        DataSourceServiceFactory _dataSourceDefinitionFactory;
        private readonly Lazy<IProviderContext> _providerContext;

        public ProviderDataSourceServiceRegistry(DataSourceServiceFactory dataSourceDefinitionFactory, Lazy<IProviderContext> providerContext)
        {
            _dataSourceDefinitionFactory = dataSourceDefinitionFactory ?? throw new ArgumentNullException(nameof(dataSourceDefinitionFactory));
            _providerContext = providerContext ?? throw new ArgumentNullException(nameof(providerContext));
        }

        private ProviderDataSourceService UpgradeDataSource(DataSourceServiceDescriptor dataSourceDescriptor)
        {
            var dataSource = _dataSourceDefinitionFactory.Build(dataSourceDescriptor);
            var dataSourceName = dataSource.Implementation.Name;
            var fullDataSourceName = $"{_providerContext.Value.SnakeCaseProviderName}_{dataSourceName}";
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
