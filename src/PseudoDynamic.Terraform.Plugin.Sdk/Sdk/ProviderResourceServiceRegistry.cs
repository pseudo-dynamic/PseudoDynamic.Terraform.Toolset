namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderResourceServiceRegistry : NamedTerraformServiceRegistry<ProviderResourceService>
    {
        ResourceServiceFactory _resourceDefinitionFactory;
        private readonly Lazy<IProviderContext> _providerContext;

        public ProviderResourceServiceRegistry(ResourceServiceFactory resourceDefinitionFactory, Lazy<IProviderContext> providerContext)
        {
            _resourceDefinitionFactory = resourceDefinitionFactory ?? throw new ArgumentNullException(nameof(resourceDefinitionFactory));
            _providerContext = providerContext ?? throw new ArgumentNullException(nameof(providerContext));
        }

        private ProviderResourceService UpgradeResource(ResourceServiceDescriptor resourceDescriptor)
        {
            var resource = _resourceDefinitionFactory.Build(resourceDescriptor);
            var resourceName = resource.Implementation.Name;
            var fullResourceName = $"{_providerContext.Value.SnakeCaseProviderName}_{resourceName}";
            return new ProviderResourceService(resource, fullResourceName);
        }

        public void Add(ResourceServiceDescriptor resourceDescriptor) =>
            Add(UpgradeResource(resourceDescriptor));

        /// <summary>
        /// Adds or replaces a resource described by <paramref name="resourceDescriptor"/>.
        /// </summary>
        /// <param name="resourceDescriptor"></param>
        internal void Replace(ResourceServiceDescriptor resourceDescriptor) =>
            Replace(UpgradeResource(resourceDescriptor));
    }
}
