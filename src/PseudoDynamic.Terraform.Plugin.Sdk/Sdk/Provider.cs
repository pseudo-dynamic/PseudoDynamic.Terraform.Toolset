using Microsoft.Extensions.Options;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class Provider : IProvider
    {
        public IReadOnlyDictionary<string, ResourceDefinition> ResourceDefinitions => _resourceDefinitions;
        public string FullyQualifiedProviderName { get; }

        private Dictionary<string, ResourceDefinition> _resourceDefinitions;
        private INameConvention _nameConvention;

        public Provider(IOptions<ProviderOptions> providerOptions)
        {
            var unwrappedProviderOptions = providerOptions.Value ?? throw new ArgumentNullException(nameof(providerOptions));
            _resourceDefinitions = new Dictionary<string, ResourceDefinition>();
            _nameConvention = unwrappedProviderOptions.NameConvention;
            FullyQualifiedProviderName = unwrappedProviderOptions.FullyQualifiedProviderName;

            foreach (var resourceDescriptor in unwrappedProviderOptions.ResourceDescriptors) {
                //_resourceDefinitions.Add(_nameConvention.Format(
            }
        }

        public void AddResource(ResourceDescriptor resource)
        {

        }
    }
}
