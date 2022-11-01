using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal class TerraformServiceFactory
    {
        private readonly SchemaBuilder _schemaBuilder;
        private readonly IServiceProvider _serviceProvider;

        public TerraformServiceFactory(IServiceProvider serviceProvider, SchemaBuilder schemaBuilder)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _schemaBuilder = schemaBuilder ?? throw new ArgumentNullException(nameof(schemaBuilder));
        }

        protected BlockDefinition BuildSchema(Type schemaType) =>
            _schemaBuilder.BuildBlock(schemaType);

        protected object CreateImplementation(ITerraformServiceDescriptor serviceDescriptor) =>
            serviceDescriptor.Implementation ?? ActivatorUtilities.CreateInstance(_serviceProvider, serviceDescriptor.ImplementationType);

        protected TImplementation CreateImplementation<TImplementation>(ITerraformServiceDescriptor serviceDescriptor) =>
            (TImplementation)CreateImplementation(serviceDescriptor);
    }
}
