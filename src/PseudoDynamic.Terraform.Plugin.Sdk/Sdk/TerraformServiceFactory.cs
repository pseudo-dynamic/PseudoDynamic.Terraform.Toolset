using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class TerraformServiceFactory
    {
        private readonly DynamicDefinitionResolver _dynamicResolver;
        private readonly IServiceProvider _serviceProvider;

        public TerraformServiceFactory(IServiceProvider serviceProvider, DynamicDefinitionResolver dynamicResolver)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _dynamicResolver = dynamicResolver ?? throw new ArgumentNullException(nameof(dynamicResolver));
        }

        protected BlockDefinition BuildSchema(ITerraformServiceDescriptor serviceDescriptor)
        {
            var schema = BlockBuilder.Default.BuildBlock(serviceDescriptor.SchemaType);
            _dynamicResolver.AddPreloadable(schema);
            return schema;
        }

        protected object BuildService(ITerraformServiceDescriptor serviceDescriptor) =>
            serviceDescriptor.Service ?? ActivatorUtilities.CreateInstance(_serviceProvider, serviceDescriptor.ServiceType);

        protected TService BuildService<TService>(ITerraformServiceDescriptor serviceDescriptor) =>
            (TService)BuildService(serviceDescriptor);
    }
}
