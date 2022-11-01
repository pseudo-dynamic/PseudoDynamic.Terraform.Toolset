using Microsoft.Extensions.Options;
using PseudoDynamic.Terraform.Plugin.Conventions;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using PseudoDynamic.Terraform.Plugin.Sdk.Services;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderContext : IProviderContext
    {
        public string FullyQualifiedProviderName { get; }

        /// <summary>
        /// The provider name. Represents the last "/"-separated part of <see cref="FullyQualifiedProviderName"/>.
        /// </summary>
        public string ProviderName { get; }

        /// <summary>
        /// Same as <see cref="ProviderName"/> but snake_case formatted to comply with resource and data source naming conventions.
        /// </summary>
        public string SnakeCaseProviderName { get; }

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
                    schema = schemaType != null ? BlockBuilder.Default.BuildBlock(schemaType) : BlockDefinition.Uncomputed;
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
        private ProviderService? _providerService;
        private ProviderServiceDescriptor? _providerServiceDescriptor;
        private Type? _providerMetaSchemaType;
        private BlockDefinition? _providerMetaSchema;
        private ProviderResourceServiceRegistry _resourceServiceRegistry;
        private IReadOnlyList<ResourceServiceDescriptor>? _resourceServiceDescriptors;
        private ProviderDataSourceServiceRegistry _dataSourceServiceRegistry;
        private IReadOnlyList<DataSourceServiceDescriptor>? _dataSourceServiceDescriptors;

        public ProviderContext(
            SchemaBuilder schemaBuilder,
            ProviderServiceFactory providerServiceFactory,
            ProviderResourceServiceRegistry resourceServiceRegistry,
            ProviderDataSourceServiceRegistry dataSourceServiceRegistry,
            IOptions<ProviderContextOptions> options)
        {
            _schemaBuilder = schemaBuilder ?? throw new ArgumentNullException(nameof(schemaBuilder));
            _providerServiceFactory = providerServiceFactory ?? throw new ArgumentNullException(nameof(providerServiceFactory));
            _resourceServiceRegistry = resourceServiceRegistry ?? throw new ArgumentNullException(nameof(resourceServiceRegistry));
            _dataSourceServiceRegistry = dataSourceServiceRegistry ?? throw new ArgumentNullException(nameof(dataSourceServiceRegistry));
            var unwrappedOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));

            FullyQualifiedProviderName = unwrappedOptions.FullyQualifiedProviderName;
            ProviderName = FullyQualifiedProviderName.Split("/").Last();
            SnakeCaseProviderName = SnakeCaseConvention.Default.Format(ProviderName);

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
