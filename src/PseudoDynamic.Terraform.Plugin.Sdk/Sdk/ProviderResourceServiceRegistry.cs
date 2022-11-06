using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderResourceServiceRegistry : NamedTerraformServiceRegistry<ProviderResourceService>
    {
        private readonly ResourceServiceFactory _resourceDefinitionFactory;
        private readonly IProviderServer _providerServer;

        public ProviderResourceServiceRegistry(ResourceServiceFactory resourceDefinitionFactory, IProviderServer providerServer)
        {
            _resourceDefinitionFactory = resourceDefinitionFactory ?? throw new ArgumentNullException(nameof(resourceDefinitionFactory));
            _providerServer = providerServer ?? throw new ArgumentNullException(nameof(providerServer));
        }

        private ProviderResourceService UpgradeResource(ResourceServiceDescriptor resourceDescriptor)
        {
            ResourceService resource = _resourceDefinitionFactory.Build(resourceDescriptor);
            string resourceName = resource.Implementation.Name;
            string fullResourceName = $"{_providerServer.SnakeCaseProviderName}_{resourceName}";
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
