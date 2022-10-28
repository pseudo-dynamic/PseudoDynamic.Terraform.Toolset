using AutoMapper;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Protocols.Consolidation;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderAdapter : IProviderAdapter
    {
        private readonly IProvider _provider;
        private readonly IMapper _mapper;
        private readonly TerraformDynamicMessagePackDecoder _decoder;
        private readonly ITerraformDynamicDecoder _dynamicDecoder;

        public ProviderAdapter(IProvider provider, IMapper mapper, TerraformDynamicMessagePackDecoder decoder, ITerraformDynamicDecoder dynamicDecoder)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _dynamicDecoder = dynamicDecoder ?? throw new ArgumentNullException(nameof(dynamicDecoder));
        }

        public async Task<GetProviderSchema.Response> GetProviderSchema(GetProviderSchema.Request request)
        {
            return new GetProviderSchema.Response() {
                Provider = Schema.TypeDependencyGraph.BlockDefinition.Uncomputed(),
                ProviderMeta = Schema.TypeDependencyGraph.BlockDefinition.Uncomputed(),
                ResourceSchemas = _provider.ResourceDefinitions.ToDictionary(x => x.Key, x => x.Value.Schema)
            };
        }

        public async Task<ValidateProviderConfig.Response> ValidateProviderConfig(ValidateProviderConfig.Request request) =>
            new ValidateProviderConfig.Response() {
                PreparedConfig = request.Config,
                Diagnostics = new List<Diagnostic>()
            };

        public Task<ConfigureProvider.Response> ConfigureProvider(ConfigureProvider.Request request) => throw new NotImplementedException();

        #region Data sources

        public Task<ReadDataSource.Response> ReadDataSource(ReadDataSource.Request request) => throw new NotImplementedException();

        public async Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Request request) => throw new NotImplementedException();

        #endregion

        #region Resources

        public async Task<ValidateResourceConfig.Response> ValidateResourceConfig(ValidateResourceConfig.Request request)
        {
            var resourceDefinition = _provider.ResourceDefinitions[request.TypeName];
            var resource = resourceDefinition.Resource;
            var reports = new Reports();

            var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
            var config = _decoder.Decode(request.Config.Msgpack, resourceDefinition.Schema, decodingOptions);

            var context = ValidateConfig.ContextAccessor
                .MakeGenericTypeAccessor(resourceDefinition.Schema.SourceType)
                .CreateInstance(x => x.GetPrivateInstanceActivator, config, reports, _dynamicDecoder);

            await (Task)resourceDefinition.ResourceAccessor
                .GetMethodCaller(nameof(IResource<object>.ValidateConfig))
                .Invoke(resource, new object?[] { context });

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
