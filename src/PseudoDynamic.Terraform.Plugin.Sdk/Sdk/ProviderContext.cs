using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using PseudoDynamic.Terraform.Plugin.Sdk.Services;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderContext : IProviderContext
    {
        public ProviderService ProviderService {
            get {
                var providerService = _providerService;

                if (providerService == null) {
                    var descriptor = _providerServiceDescriptor;
                    providerService = descriptor != null ? _providerServiceFactory.Build(descriptor) : _providerServiceFactory.BuildUnimplemented();
                    _providerService = providerService;
                }

                return providerService;
            }
        }

        public BlockDefinition ProviderMetaSchema {
            get {
                var schema = _providerMetaSchema;

                if (schema == null) {
                    var schemaType = _providerMetaSchemaType;
                    schema = schemaType != null ? _schemaBuilder.BuildBlock(schemaType) : BlockDefinition.Uncomputed;
                    _providerMetaSchema = schema;
                }

                return schema;
            }
        }

        public IReadOnlyDictionary<string, ProviderResourceService> ResourceServices {
            get {
                var descriptors = _resourceServiceDescriptors;

                if (descriptors != null) {
                    _resourceServiceDescriptors = null;

                    foreach (var resourceDescriptor in descriptors) {
                        _resourceServiceRegistry.Add(resourceDescriptor);
                    }
                }

                return _resourceServiceRegistry.Services;
            }
        }

        public IReadOnlyDictionary<string, ProviderDataSourceService> DataSourceServices {
            get {
                var descriptors = _dataSourceServiceDescriptors;

                if (descriptors != null) {
                    _dataSourceServiceDescriptors = null;

                    foreach (var dataSourceDescriptor in descriptors) {
                        _dataSourceServiceRegistry.Add(dataSourceDescriptor);
                    }
                }

                return _dataSourceServiceRegistry.Services;
            }
        }

        private readonly SchemaBuilder _schemaBuilder;
        private readonly ProviderServiceFactory _providerServiceFactory;
        private readonly ProviderResourceServiceRegistry _resourceServiceRegistry;
        private readonly ProviderDataSourceServiceRegistry _dataSourceServiceRegistry;
        private ProviderService? _providerService;
        private readonly ProviderServiceDescriptor? _providerServiceDescriptor;
        private readonly Type? _providerMetaSchemaType;
        private BlockDefinition? _providerMetaSchema;
        private IReadOnlyList<ResourceServiceDescriptor>? _resourceServiceDescriptors;
        private IReadOnlyList<DataSourceServiceDescriptor>? _dataSourceServiceDescriptors;

        public ProviderContext(
            SchemaBuilder schemaBuilder,
            ProviderServiceFactory providerServiceFactory,
            ProviderResourceServiceRegistry resourceServiceRegistry,
            ProviderDataSourceServiceRegistry dataSourceServiceRegistry,
            IOptions<ProviderOptions> options)
        {
            _schemaBuilder = schemaBuilder ?? throw new ArgumentNullException(nameof(schemaBuilder));
            _providerServiceFactory = providerServiceFactory ?? throw new ArgumentNullException(nameof(providerServiceFactory));
            _resourceServiceRegistry = resourceServiceRegistry ?? throw new ArgumentNullException(nameof(resourceServiceRegistry));
            _dataSourceServiceRegistry = dataSourceServiceRegistry ?? throw new ArgumentNullException(nameof(dataSourceServiceRegistry));
            var unwrappedOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));

            // Service descriptors and schema evaluations must be deferred until first Terraform
            // initiated gRPC remote call to allow the user having a detailed error message on
            // failure
            _providerServiceDescriptor = unwrappedOptions.ProviderDescriptor;
            _providerMetaSchemaType = unwrappedOptions.ProviderSchemaType;
            _resourceServiceDescriptors = unwrappedOptions.ResourceDescriptors;
            _dataSourceServiceDescriptors = unwrappedOptions.DataSourceDescriptors;
        }

        void IProviderContext.ReplaceResource(ResourceServiceDescriptor resourceDescriptor) =>
            _resourceServiceRegistry.Replace(resourceDescriptor);
    }
}
