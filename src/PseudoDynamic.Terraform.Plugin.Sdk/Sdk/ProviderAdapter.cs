using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using PseudoDynamic.Terraform.Plugin.Protocols.Consolidated;
using PseudoDynamic.Terraform.Plugin.Sdk.Services;
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

        #region Provider

        public async Task<GetProviderSchema.Response> GetProviderSchema(GetProviderSchema.Request request) =>
            new GetProviderSchema.Response() {
                Provider = _provider.ProviderService.Schema,
                ProviderMeta = _provider.ProviderMetaSchema,
                ResourceSchemas = _provider.ResourceServices.ToDictionary(x => x.Key, x => x.Value.Schema),
                DataSourceSchemas = _provider.DataSourceServices.ToDictionary(x => x.Key, x => x.Value.Schema)
            };

        public Task<ValidateProviderConfig.Response> ValidateProviderConfig(ValidateProviderConfig.Request request)
        {
            var service = _provider.ProviderService;
            return service.Implementation.ProviderAdapter.ValidateProviderConfig(this, service, request);
        }

        public Task<ConfigureProvider.Response> ConfigureProvider(ConfigureProvider.Request request)
        {
            var service = _provider.ProviderService;

            if (!service.HasImplementation) {
                return Task.FromResult(new ConfigureProvider.Response());
            }

            return service.Implementation.ProviderAdapter.ConfigureProvider(this, service, request);
        }

        #endregion

        #region Resources

        public Task<UpgradeResourceState.Response> UpgradeResourceState(UpgradeResourceState.Request request)
        {
            var service = _provider.ResourceServices[request.TypeName];
            return service.Implementation.ResourceAdapter.UpgradeResourceState(this, service, request);
        }

        public Task<ImportResourceState.Response> ImportResourceState(ImportResourceState.Request request) => throw new NotImplementedException();

        public Task<ReadResource.Response> ReadResource(ReadResource.Request request)
        {
            var service = _provider.ResourceServices[request.TypeName];
            return service.Implementation.ResourceAdapter.ReadResource(this, service, request);
        }

        public Task<ValidateResourceConfig.Response> ValidateResourceConfig(ValidateResourceConfig.Request request)
        {
            var service = _provider.ResourceServices[request.TypeName];
            return service.Implementation.ResourceAdapter.ValidateResourceConfig(this, service, request);
        }

        public Task<PlanResourceChange.Response> PlanResourceChange(PlanResourceChange.Request request)
        {
            var service = _provider.ResourceServices[request.TypeName];
            return service.Implementation.ResourceAdapter.PlanResourceChange(this, service, request);
        }

        public Task<ApplyResourceChange.Response> ApplyResourceChange(ApplyResourceChange.Request request)
        {
            var service = _provider.ResourceServices[request.TypeName];
            return service.Implementation.ResourceAdapter.ApplyResourceChange(this, service, request);
        }

        #endregion

        #region Data Sources

        public Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Request request)
        {
            var service = _provider.DataSourceServices[request.TypeName];
            return service.Implementation.DataSourceAdapter.ValidateDataResourceConfig(this, service, request);
        }

        public Task<ReadDataSource.Response> ReadDataSource(ReadDataSource.Request request)
        {
            var service = _provider.DataSourceServices[request.TypeName];
            return service.Implementation.DataSourceAdapter.ReadDataSource(this, service, request);
        }

        #endregion

        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "False positive!?!?!?!?")]
        [SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration.", Justification = "False positive")]
        private void Deconstruct(
            out IProviderContext provider,
            out IMapper mapper,
            out TerraformDynamicMessagePackDecoder decoder,
            out ITerraformDynamicDecoder dynamicDecoder,
            out TerraformDynamicMessagePackEncoder encoder)
        {
            provider = _provider;
            mapper = _mapper;
            decoder = _decoder;
            dynamicDecoder = _dynamicDecoder;
            encoder = _encoder;
        }

        public Task<StopProvider.Response> StopProvider(StopProvider.Request request) => throw new NotImplementedException();

        internal interface IProviderAdapter
        {
            Task<ValidateProviderConfig.Response> ValidateProviderConfig(ProviderAdapter adapter, ProviderService service, ValidateProviderConfig.Request request);
            Task<ConfigureProvider.Response> ConfigureProvider(ProviderAdapter adapter, ProviderService service, ConfigureProvider.Request request);
        }

        internal readonly struct ProviderGenericAdapter<Schema> : IProviderAdapter
            where Schema : class
        {
            public async Task<ValidateProviderConfig.Response> ValidateProviderConfig(ProviderAdapter adapter, ProviderService service, ValidateProviderConfig.Request request)
            {
                var (_, mapper, decoder, dynamicDecoder, _) = adapter;
                var provider = (IProvider<Schema>)service.Implementation;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var config = (Schema)decoder.DecodeBlock(request.Config.Msgpack, service.Schema, decodingOptions);
                var context = new Provider.ValidateConfigContext<Schema>(reports, dynamicDecoder, config);
                await provider.ValidateConfig(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);

                return new ValidateProviderConfig.Response() {
                    Diagnostics = diagnostics
                };
            }

            public async Task<ConfigureProvider.Response> ConfigureProvider(ProviderAdapter adapter, ProviderService service, ConfigureProvider.Request request)
            {
                var (_, mapper, decoder, dynamicDecoder, _) = adapter;
                var provider = (IProvider<Schema>)service.Implementation!;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var config = (Schema)decoder.DecodeBlock(request.Config.Msgpack, service.Schema, decodingOptions);
                var context = new Provider.ConfigureContext<Schema>(reports, dynamicDecoder, config);
                await provider.Configure(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);

                return new ConfigureProvider.Response() {
                    Diagnostics = diagnostics
                };
            }
        }

        internal interface IResourceAdapter
        {
            Task<UpgradeResourceState.Response> UpgradeResourceState(ProviderAdapter adapter, ProviderResourceService service, UpgradeResourceState.Request request);
            Task<ImportResourceState.Response> ImportResourceState(ProviderAdapter adapter, ProviderResourceService service, ImportResourceState.Request request);
            Task<ReadResource.Response> ReadResource(ProviderAdapter adapter, ProviderResourceService service, ReadResource.Request request);
            Task<ValidateResourceConfig.Response> ValidateResourceConfig(ProviderAdapter adapter, ProviderResourceService service, ValidateResourceConfig.Request request);
            Task<PlanResourceChange.Response> PlanResourceChange(ProviderAdapter adapter, ProviderResourceService service, PlanResourceChange.Request request);
            Task<ApplyResourceChange.Response> ApplyResourceChange(ProviderAdapter adapter, ProviderResourceService service, ApplyResourceChange.Request request);
        }

        internal readonly struct ResourceGenericAdapter<Schema, ProviderMetaSchema> : IResourceAdapter
            where Schema : class
            where ProviderMetaSchema : class
        {
            public async Task<UpgradeResourceState.Response> UpgradeResourceState(ProviderAdapter adapter, ProviderResourceService service, UpgradeResourceState.Request request)
            {
                var (_, mapper, _, _, _) = adapter;
                var resource = (IResource<Schema, ProviderMetaSchema>)service.Implementation;
                var reports = new Reports();
                var context = new Resource.MigrateStateContext(reports, request.Version);
                await resource.MigrateState(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);

                return new UpgradeResourceState.Response() {
                    UpgradedState = DynamicValue.OfJson(request.RawState.Json),
                    Diagnostics = diagnostics
                };
            }

            public Task<ImportResourceState.Response> ImportResourceState(ProviderAdapter adapter, ProviderResourceService service, ImportResourceState.Request request) => throw new NotImplementedException();

            public async Task<ReadResource.Response> ReadResource(ProviderAdapter adapter, ProviderResourceService service, ReadResource.Request request)
            {
                var (provider, mapper, decoder, dynamicDecoder, encoder) = adapter;
                var resource = (IResource<Schema, ProviderMetaSchema>)service.Implementation;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var decodedState = (Schema)decoder.DecodeBlock(request.CurrentState.Msgpack, service.Schema, decodingOptions);
                var decodedProviderMeta = (ProviderMetaSchema)decoder.DecodeNullableBlock(request.ProviderMeta.Msgpack, provider.ProviderMetaSchema, decodingOptions)!;
                var context = new Resource.ReviseStateContext<Schema, ProviderMetaSchema>(reports, dynamicDecoder, decodedState, decodedProviderMeta);
                await resource.ReviseState(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);
                var encodedState = encoder.EncodeBlock(service.Schema, context.State);

                return new ReadResource.Response() {
                    NewState = DynamicValue.OfMessagePack(encodedState),
                    Diagnostics = diagnostics
                };
            }

            public async Task<ValidateResourceConfig.Response> ValidateResourceConfig(ProviderAdapter adapter, ProviderResourceService service, ValidateResourceConfig.Request request)
            {
                var (_, mapper, decoder, dynamicDecoder, _) = adapter;
                var resource = (IResource<Schema, ProviderMetaSchema>)service.Implementation;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var decodedConfig = (Schema)decoder.DecodeBlock(request.Config.Msgpack, service.Schema, decodingOptions);
                var context = new Resource.ValidateConfigContext<Schema>(reports, dynamicDecoder, decodedConfig);
                await resource.ValidateConfig(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);

                return new ValidateResourceConfig.Response() {
                    Diagnostics = diagnostics
                };
            }

            public async Task<PlanResourceChange.Response> PlanResourceChange(ProviderAdapter adapter, ProviderResourceService service, PlanResourceChange.Request request)
            {
                var (provider, mapper, decoder, dynamicDecoder, encoder) = adapter;
                var resource = (IResource<Schema, ProviderMetaSchema>)service.Implementation;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var decodedConfig = (Schema)decoder.DecodeBlock(request.Config.Msgpack, service.Schema, decodingOptions);
                var decodedState = (Schema?)decoder.DecodeNullableBlock(request.PriorState.Msgpack, service.Schema, decodingOptions);
                var decodedPlan = (Schema?)decoder.DecodeNullableBlock(request.ProposedNewState.Msgpack, service.Schema, decodingOptions);
                var decodedProviderMeta = (ProviderMetaSchema)decoder.DecodeNullableBlock(request.ProviderMeta.Msgpack, provider.ProviderMetaSchema, decodingOptions)!;

                var context = new Resource.PlanContext<Schema, ProviderMetaSchema>(
                    reports,
                    dynamicDecoder,
                    config: decodedConfig,
                    state: decodedState,
                    plan: decodedPlan,
                    decodedProviderMeta);

                await resource.Plan(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);
                var encodedPlan = encoder.EncodeValue(service.Schema, context.Plan);

                return new PlanResourceChange.Response() {
                    Diagnostics = diagnostics,
                    PlannedState = DynamicValue.OfMessagePack(encodedPlan)
                };
            }

            public async Task<ApplyResourceChange.Response> ApplyResourceChange(ProviderAdapter adapter, ProviderResourceService service, ApplyResourceChange.Request request)
            {
                var (provider, mapper, decoder, dynamicDecoder, encoder) = adapter;
                var resource = (IResource<Schema, ProviderMetaSchema>)service.Implementation;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var decodedConfig = (Schema)decoder.DecodeBlock(request.Config.Msgpack, service.Schema, decodingOptions);
                var decodedState = (Schema?)decoder.DecodeNullableBlock(request.PriorState.Msgpack, service.Schema, decodingOptions);
                var decodedPlan = (Schema?)decoder.DecodeNullableBlock(request.PlannedState.Msgpack, service.Schema, decodingOptions);
                var decodedProviderMeta = (ProviderMetaSchema)decoder.DecodeNullableBlock(request.ProviderMeta.Msgpack, provider.ProviderMetaSchema, decodingOptions)!;

                var context = new Resource.PlanContext<Schema, ProviderMetaSchema>(
                    reports,
                    dynamicDecoder,
                    config: decodedConfig,
                    state: decodedState,
                    plan: decodedPlan,
                    decodedProviderMeta);

                await resource.Apply(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);
                var encodedPlan = encoder.EncodeValue(service.Schema, context.Plan);

                return new ApplyResourceChange.Response() {
                    Diagnostics = diagnostics,
                    NewState = DynamicValue.OfMessagePack(encodedPlan)
                };
            }
        }

        internal interface IDataSourceAdapter
        {
            Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ProviderAdapter adapter, ProviderDataSourceService service, ValidateDataResourceConfig.Request request);
            Task<ReadDataSource.Response> ReadDataSource(ProviderAdapter adapter, ProviderDataSourceService service, ReadDataSource.Request request);
        }

        internal readonly struct DataSourceGenericAdapter<Schema, ProviderMetaSchema> : IDataSourceAdapter
            where Schema : class
            where ProviderMetaSchema : class
        {
            public async Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ProviderAdapter adapter, ProviderDataSourceService service, ValidateDataResourceConfig.Request request)
            {
                var (_, mapper, decoder, dynamicDecoder, _) = adapter;
                var dataSource = (IDataSource<Schema, ProviderMetaSchema>)service.Implementation;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var config = (Schema)decoder.DecodeBlock(request.Config.Msgpack, service.Schema, decodingOptions);
                var context = new DataSource.ValidateConfigContext<Schema>(reports, dynamicDecoder, config);
                await dataSource.ValidateConfig(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);

                return new ValidateDataResourceConfig.Response() {
                    Diagnostics = diagnostics
                };
            }

            public async Task<ReadDataSource.Response> ReadDataSource(ProviderAdapter adapter, ProviderDataSourceService service, ReadDataSource.Request request)
            {
                var (provider, mapper, decoder, dynamicDecoder, encoder) = adapter;
                var dataSource = (IDataSource<Schema, ProviderMetaSchema>)service.Implementation;
                var reports = new Reports();
                var decodingOptions = new TerraformDynamicMessagePackDecoder.DecodingOptions() { Reports = reports };
                var decodedState = (Schema)decoder.DecodeBlock(request.Config.Msgpack, service.Schema, decodingOptions);
                var decodedProviderMeta = (ProviderMetaSchema)decoder.DecodeNullableBlock(request.ProviderMeta.Msgpack, provider.ProviderMetaSchema, decodingOptions)!;
                var context = new DataSource.ReadContext<Schema, ProviderMetaSchema>(reports, dynamicDecoder, decodedState, decodedProviderMeta);
                await dataSource.Read(context);
                var diagnostics = mapper.Map<IList<Diagnostic>>(reports);
                var encodedState = encoder.EncodeValue(service.Schema, decodedState);

                return new ReadDataSource.Response() {
                    Diagnostics = diagnostics,
                    State = DynamicValue.OfMessagePack(encodedState)
                };
            }
        }
    }
}
