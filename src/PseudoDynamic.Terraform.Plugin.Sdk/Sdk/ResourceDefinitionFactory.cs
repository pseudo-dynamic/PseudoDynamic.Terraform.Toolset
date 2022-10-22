using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ResourceDefinitionFactory
    {
        IServiceProvider _serviceProvider;

        public ResourceDefinitionFactory(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public ResourceDefinition Build(ResourceDescriptor resourceDescriptor)
        {
            var schema = BlockBuilder.Default.BuildBlock(resourceDescriptor.SchemaType);
            var resource = resourceDescriptor.Resource ?? ActivatorUtilities.CreateInstance(_serviceProvider, resourceDescriptor.ResourceType);
            var resourceInfo = (IResourceInfo)resource;
            return new ResourceDefinition(schema, resourceInfo);
        }
    }
}
