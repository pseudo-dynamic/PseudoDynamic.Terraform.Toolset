using System.Data;
using Microsoft.Extensions.Options;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class Provider : IProvider
    {
        public IReadOnlyDictionary<string, ProviderResourceDefinition> ResourceDefinitions => _resourceDefinitions;
        public string FullyQualifiedProviderName { get; }

        /// <summary>
        /// The provider name. Represents the last "/"-separated part of <see cref="FullyQualifiedProviderName"/>.
        /// </summary>
        public string ProviderName { get; }

        /// <summary>
        /// Same as <see cref="ProviderName"/> but snake_case formatted to comply with resource and data source naming conventions.
        /// </summary>
        public string SnakeCaseProviderName { get; }

        private readonly Dictionary<string, ProviderResourceDefinition> _resourceDefinitions;
        private readonly ResourceDefinitionFactory _resourceDefinitionFactory;

        public Provider(ResourceDefinitionFactory resourceDefinitionFactory, IOptionsSnapshot<ProviderOptions> providerOptions)
        {
            var unwrappedProviderOptions = providerOptions?.Value ?? throw new ArgumentNullException(nameof(providerOptions));
            _resourceDefinitions = new Dictionary<string, ProviderResourceDefinition>();

            FullyQualifiedProviderName = unwrappedProviderOptions.FullyQualifiedProviderName;
            ProviderName = FullyQualifiedProviderName.Split("/").Last();
            SnakeCaseProviderName = SnakeCaseConvention.Default.Format(ProviderName);
            _resourceDefinitionFactory = resourceDefinitionFactory;

            foreach (var resourceDescriptor in unwrappedProviderOptions.ResourceDescriptors) {
                AddResourceDefinition(resourceDescriptor);
            }
        }

        private ProviderResourceDefinition UpgradeResourceDefinition(ResourceDescriptor resourceDescriptor)
        {
            var resourceDefinition = _resourceDefinitionFactory.Build(resourceDescriptor);
            var resourceTypeName = resourceDefinition.Resource.TypeName;
            var fullResourceTypeName = $"{SnakeCaseProviderName}_{resourceTypeName}";
            return new ProviderResourceDefinition(resourceDefinition, fullResourceTypeName);
        }

        private void AddResourceDefinition(ResourceDescriptor resourceDescriptor)
        {
            var resourceDefinition = UpgradeResourceDefinition(resourceDescriptor);
            var resourceTypeName = resourceDefinition.ResourceTypeName;

            if (_resourceDefinitions.ContainsKey(resourceTypeName)) {
                throw new DuplicateNameException($"The resource type name \"{resourceTypeName}\" is already taken");
            }

            _resourceDefinitions.Add(resourceTypeName, resourceDefinition);
        }

        /// <summary>
        /// Adds or replaces a resource described by <paramref name="resourceDescriptor"/>.
        /// </summary>
        /// <param name="resourceDescriptor"></param>
        internal void ReplaceResourceDefinition(ResourceDescriptor resourceDescriptor)
        {
            var resourceDefinition = UpgradeResourceDefinition(resourceDescriptor);
            _resourceDefinitions[resourceDefinition.ResourceTypeName] = resourceDefinition;
        }

        void IProvider.ReplaceResourceDefinition(ResourceDescriptor resourceDescriptor) =>
            ReplaceResourceDefinition(resourceDescriptor);
    }
}
