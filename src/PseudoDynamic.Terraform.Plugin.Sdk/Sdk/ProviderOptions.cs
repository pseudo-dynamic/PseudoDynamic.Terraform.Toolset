using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderOptions
    {
        private static void ValidateProviderName([NotNull] string? providerName)
        {
            if (string.IsNullOrEmpty(providerName)) {
                throw new TerraformNameConventionException("Fully-qualified provider name is null or empty");
            }

            TerraformNameConventionException.EnsureProviderNameConvention(providerName);
        }

        public List<ResourceDescriptor> ResourceDescriptors { get; } = new List<ResourceDescriptor>();

        internal IReadOnlyList<ResourceDefinition> ResourceDefinitions => _resourceDefinitions;

        public string FullyQualifiedProviderName {
            get {
                var providerName = _fullyQualifiedProviderName;
                ValidateProviderName(providerName);
                return providerName;
            }

            set {
                ValidateProviderName(value);
                _fullyQualifiedProviderName = value;
            }
        }

        private string? _fullyQualifiedProviderName;
        private readonly List<ResourceDefinition> _resourceDefinitions = new List<ResourceDefinition>();

        public class RequestResources : IPostConfigureOptions<ProviderOptions>
        {
            private readonly IServiceProvider _serviceProvider;

            public RequestResources(IServiceProvider serviceProvider) =>
                _serviceProvider = serviceProvider;

            private ResourceDefinition BuildResourceDefinition(ResourceDescriptor resourceDescriptor)
            {
                var schema = BlockBuilder.Default.BuildBlock(resourceDescriptor.SchemaType);
                var resource = resourceDescriptor.Resource ?? ActivatorUtilities.CreateInstance(_serviceProvider, resourceDescriptor.ResourceType);
                var resourceInfo = (IResourceInfo)resource;
                return new ResourceDefinition(schema, resourceInfo);
            }

            public void PostConfigure(string name, ProviderOptions options)
            {
                foreach (var resourceDescriptor in options.ResourceDescriptors) {
                    var resourceDefinition = BuildResourceDefinition(resourceDescriptor);
                    options._resourceDefinitions.Add(resourceDefinition);
                }
            }
        }
    }
}
