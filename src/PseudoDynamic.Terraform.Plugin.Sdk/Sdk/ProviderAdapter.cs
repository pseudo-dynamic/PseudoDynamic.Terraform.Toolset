using AutoMapper;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Protocols.Models;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.MessagePack;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderAdapter : IProviderAdapter
    {
        public static readonly IResource<object>? ResourceDummy;

        private readonly IProvider _provider;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public ProviderAdapter(IProvider provider, IMapper mapper, IServiceProvider serviceProvider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<GetProviderSchema.Response> GetProviderSchema(GetProviderSchema.Request request)
        {
            return new GetProviderSchema.Response() {
                Provider = Schema.TypeDependencyGraph.BlockDefinition.Uncomputed(),
                ProviderMeta = Schema.TypeDependencyGraph.BlockDefinition.Uncomputed(),
                ResourceSchemas = _provider.ResourceDefinitions.ToDictionary(x => x.Key, x => x.Value.Schema)
            };
        }

        public async Task<ValidateProviderConfig.Response> ValidateProviderConfig(ValidateProviderConfig.Request request) => throw new NotImplementedException();

        public Task<ConfigureProvider.Response> ConfigureProvider(ConfigureProvider.Request request) => throw new NotImplementedException();

        #region Data sources

        public Task<ReadDataSource.Response> ReadDataSource(ReadDataSource.Request request) => throw new NotImplementedException();

        public async Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Request request) => throw new NotImplementedException();

        #endregion

        #region resources

        public async Task<ValidateResourceConfig.Response> ValidateResourceConfig(ValidateResourceConfig.Request request)
        {
            var resourceDefinition = _provider.ResourceDefinitions[request.TypeName];
            var resource = resourceDefinition.Resource;
            var reports = new Reports();

            var decodingOptions = new DynamicValueDecoder.DecodingOptions() { Reports = reports };
            var schema = new DynamicValueDecoder(_serviceProvider).DecodeSchema(resourceDefinition.Schema, request.Config!.Msgpack!.Value, decodingOptions);

            var context = ValidateConfig.ContextAccessor.CreateInstance(resourceDefinition.Schema.SourceType, schema, reports);
            resourceDefinition.ResourceAccessor.GetMethod(nameof(ResourceDummy.ValidateConfig)).Invoke(resource, new object?[] { context });

            var diagnostics = _mapper.Map<IList<Diagnostic>>(reports);

            return new ValidateResourceConfig.Response() {
                Diagnostics = diagnostics
            };
        }

        public Task<ReadResource.Response> ReadResource(ReadResource.Request request) => throw new NotImplementedException();

        public Task<PlanResourceChange.Response> PlanResourceChange(PlanResourceChange.Request request) => throw new NotImplementedException();

        public Task<ApplyResourceChange.Response> ApplyResourceChange(ApplyResourceChange.Request request) => throw new NotImplementedException();

        public Task<UpgradeResourceState.Response> UpgradeResourceState(UpgradeResourceState.Request request) => throw new NotImplementedException();

        public Task<ImportResourceState.Response> ImportResourceState(ImportResourceState.Request request) => throw new NotImplementedException();

        #endregion

        public Task<StopProvider.Response> StopProvider(StopProvider.Request request) => throw new NotImplementedException();
    }
}
