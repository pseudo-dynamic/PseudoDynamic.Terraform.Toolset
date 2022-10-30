using AutoMapper;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Protocols.Consolidated;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderAdapter : IProviderAdapter
    {
        private readonly IProviderContext _provider;
        private readonly IMapper _mapper;
        private readonly TerraformDynamicMessagePackDecoder _decoder;
        private readonly ITerraformDynamicDecoder _dynamicDecoder;
        private readonly TerraformDynamicMessagePackEncoder _encoder;

        public ProviderAdapter(
            IProviderContext provider,
            IMapper mapper,
            TerraformDynamicMessagePackDecoder decoder,
            ITerraformDynamicDecoder dynamicDecoder,
            TerraformDynamicMessagePackEncoder encoder)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _dynamicDecoder = dynamicDecoder ?? throw new ArgumentNullException(nameof(dynamicDecoder));
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public async Task<GetProviderSchema.Response> GetProviderSchema(GetProviderSchema.Request request)
        {
            return new GetProviderSchema.Response() {
                Provider = _provider.ProviderService?.Schema ?? Schema.TypeDependencyGraph.BlockDefinition.Uncomputed(),
                ProviderMeta = Schema.TypeDependencyGraph.BlockDefinition.Uncomputed(),
                ResourceSchemas = _provider.ResourceServices.ToDictionary(x => x.Key, x => x.Value.Schema),
                DataSourceSchemas = _provider.DataSourceServices.ToDictionary(x => x.Key, x => x.Value.Schema)
            };
        }

        public async Task<ValidateProviderConfig.Response> ValidateProviderConfig(ValidateProviderConfig.Request request) =>
            new ValidateProviderConfig.Response() {
                PreparedConfig = request.Config,
                Diagnostics = new List<Diagnostic>()
            };

        public async Task<ConfigureProvider.Response> ConfigureProvider(ConfigureProvider.Request request)
        {
            var providerService = _provider.ProviderService;

            if (providerService == null) {
                return new ConfigureProvider.Response();
            }

            var service = providerService.Service;
            var reports = new Reports();

            var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
            var config = _decoder.DecodeBlock(request.Config.Msgpack, providerService.Schema, decodingOptions);

            var context = Provider.ConfigureContextAccessor
                .MakeGenericTypeAccessor(providerService.Schema.SourceType)
                .CreateInstance(x => x.GetPrivateInstanceActivator, reports, _dynamicDecoder, config);

            await (Task)providerService.ServiceTypeAccessor
                .GetMethodCaller(nameof(IProvider<object>.Configure))
                .Invoke(service, new object?[] { context });

            var diagnostics = _mapper.Map<IList<Diagnostic>>(reports);

            return new ConfigureProvider.Response() {
                Diagnostics = diagnostics
            };
        }

        #region Data sources

        public async Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Request request)
        {
            var dataSourceDefinition = _provider.DataSourceServices[request.TypeName];
            var dataSource = dataSourceDefinition.Service;
            var reports = new Reports();

            var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
            var config = _decoder.DecodeBlock(request.Config.Msgpack, dataSourceDefinition.Schema, decodingOptions);

            var context = DataSource.ValidateContextAccessor
                .MakeGenericTypeAccessor(dataSourceDefinition.Schema.SourceType)
                .CreateInstance(x => x.GetPrivateInstanceActivator, reports, _dynamicDecoder, config);

            await (Task)dataSourceDefinition.ServiceTypeAccessor
                .GetMethodCaller(nameof(IDataSource<object>.ValidateConfig))
                .Invoke(dataSource, new object?[] { context });

            var diagnostics = _mapper.Map<IList<Diagnostic>>(reports);

            return new ValidateDataResourceConfig.Response() {
                Diagnostics = diagnostics
            };
        }

        public async Task<ReadDataSource.Response> ReadDataSource(ReadDataSource.Request request)
        {
            var dataSourceDefinition = _provider.DataSourceServices[request.TypeName];
            var dataSource = dataSourceDefinition.Service;
            var reports = new Reports();

            var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
            var decodedComputed = _decoder.DecodeBlock(request.Config.Msgpack, dataSourceDefinition.Schema, decodingOptions);

            var context = DataSource.ReadContextAccessor
                .MakeGenericTypeAccessor(dataSourceDefinition.Schema.SourceType)
                .CreateInstance(x => x.GetPrivateInstanceActivator, reports, _dynamicDecoder, decodedComputed);

            await (Task)dataSourceDefinition.ServiceTypeAccessor
                .GetMethodCaller(nameof(IDataSource<object>.Read))
                .Invoke(dataSource, new object?[] { context });

            var diagnostics = _mapper.Map<IList<Diagnostic>>(reports);
            var encodedComputed = _encoder.EncodeValue(dataSourceDefinition.Schema, decodedComputed);

            return new ReadDataSource.Response() {
                Diagnostics = diagnostics,
                State = DynamicValue.OfMessagePack(encodedComputed)
            };
        }

        #endregion

        #region Resources

        public Task<UpgradeResourceState.Response> UpgradeResourceState(UpgradeResourceState.Request request) => throw new NotImplementedException();

        public Task<ImportResourceState.Response> ImportResourceState(ImportResourceState.Request request) => throw new NotImplementedException();

        public Task<ReadResource.Response> ReadResource(ReadResource.Request request) => throw new NotImplementedException();

        public async Task<ValidateResourceConfig.Response> ValidateResourceConfig(ValidateResourceConfig.Request request)
        {
            var resourceDefinition = _provider.ResourceServices[request.TypeName];
            var resource = resourceDefinition.Service;
            var reports = new Reports();

            var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
            var config = _decoder.DecodeBlock(request.Config.Msgpack, resourceDefinition.Schema, decodingOptions);

            var context = Resource.ValidateContextAccessor
                .MakeGenericTypeAccessor(resourceDefinition.Schema.SourceType)
                .CreateInstance(x => x.GetPrivateInstanceActivator, reports, _dynamicDecoder, config);

            await (Task)resourceDefinition.ServiceTypeAccessor
                .GetMethodCaller(nameof(IResource<object>.ValidateConfig))
                .Invoke(resource, new object?[] { context });

            var diagnostics = _mapper.Map<IList<Diagnostic>>(reports);

            return new ValidateResourceConfig.Response() {
                Diagnostics = diagnostics
            };
        }

        public async Task<PlanResourceChange.Response> PlanResourceChange(PlanResourceChange.Request request)
        {
            var resourceDefinition = _provider.ResourceServices[request.TypeName];
            var resource = resourceDefinition.Service;
            var reports = new Reports();

            var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
            var decodedPlanned = _decoder.DecodeBlock(request.ProposedNewState.Msgpack, resourceDefinition.Schema, decodingOptions);
            var decodedConfig = _decoder.DecodeBlock(request.Config.Msgpack, resourceDefinition.Schema, decodingOptions);

            var context = Resource.PlanContextAccessor
                .MakeGenericTypeAccessor(resourceDefinition.Schema.SourceType)
                .CreateInstance(x => x.GetPrivateInstanceActivator, reports, _dynamicDecoder, decodedConfig, decodedPlanned);

            await (Task)resourceDefinition.ServiceTypeAccessor
                .GetMethodCaller(nameof(IResource<object>.Plan))
                .Invoke(resource, new object?[] { context });

            var diagnostics = _mapper.Map<IList<Diagnostic>>(reports);
            var encodedPlanned = _encoder.EncodeValue(resourceDefinition.Schema, decodedPlanned);

            return new PlanResourceChange.Response() {
                Diagnostics = diagnostics,
                PlannedState = DynamicValue.OfMessagePack(encodedPlanned)
            };
        }

        public Task<ApplyResourceChange.Response> ApplyResourceChange(ApplyResourceChange.Request request) => throw new NotImplementedException();

        #endregion

        public Task<StopProvider.Response> StopProvider(StopProvider.Request request) => throw new NotImplementedException();
    }
}
