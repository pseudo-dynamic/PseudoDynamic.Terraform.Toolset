using AutoMapper;
using Grpc.Core;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class ProviderAdapter : Provider.ProviderBase
    {
        private readonly IProviderAdapter _providerAdapter;
        private readonly IMapper _mapper;

        public ProviderAdapter(IProviderAdapter providerAdapter, IMapper mapper)
        {
            _providerAdapter = providerAdapter;
            _mapper = mapper;
        }

        public override async Task<GetProviderSchema.Types.Response> GetSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.GetProviderSchema.Request>(request);
            var response = await _providerAdapter.GetProviderSchema(mappedRequest);
            return _mapper.Map<GetProviderSchema.Types.Response>(response);
        }

        public override async Task<Configure.Types.Response> Configure(Configure.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ConfigureProvider.Request>(request);
            var response = await _providerAdapter.ConfigureProvider(mappedRequest);
            return _mapper.Map<Configure.Types.Response>(response);
        }

        public override async Task<PrepareProviderConfig.Types.Response> PrepareProviderConfig(PrepareProviderConfig.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ValidateProviderConfig.Request>(request);
            var response = await _providerAdapter.ValidateProviderConfig(mappedRequest);
            return _mapper.Map<PrepareProviderConfig.Types.Response>(response);
        }

        public override async Task<ValidateDataSourceConfig.Types.Response> ValidateDataSourceConfig(ValidateDataSourceConfig.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ValidateDataResourceConfig.Request>(request);
            var response = await _providerAdapter.ValidateDataResourceConfig(mappedRequest);
            return _mapper.Map<ValidateDataSourceConfig.Types.Response>(response);
        }

        public override async Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ReadDataSource.Request>(request);
            var response = await _providerAdapter.ReadDataSource(mappedRequest);
            return _mapper.Map<ReadDataSource.Types.Response>(response);
        }

        public override async Task<ValidateResourceTypeConfig.Types.Response> ValidateResourceTypeConfig(ValidateResourceTypeConfig.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ValidateResourceConfig.Request>(request);
            var response = await _providerAdapter.ValidateResourceConfig(mappedRequest);
            return _mapper.Map<ValidateResourceTypeConfig.Types.Response>(response);
        }

        public override async Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ReadResource.Request>(request);
            var response = await _providerAdapter.ReadResource(mappedRequest);
            return _mapper.Map<ReadResource.Types.Response>(response);
        }

        public override async Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.PlanResourceChange.Request>(request);
            var response = await _providerAdapter.PlanResourceChange(mappedRequest);
            return _mapper.Map<PlanResourceChange.Types.Response>(response);
        }

        public override async Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ApplyResourceChange.Request>(request);
            var response = await _providerAdapter.ApplyResourceChange(mappedRequest);
            return _mapper.Map<ApplyResourceChange.Types.Response>(response);
        }

        public override async Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.UpgradeResourceState.Request>(request);
            var response = await _providerAdapter.UpgradeResourceState(mappedRequest);
            return _mapper.Map<UpgradeResourceState.Types.Response>(response);
        }

        public override async Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.ImportResourceState.Request>(request);
            var response = await _providerAdapter.ImportResourceState(mappedRequest);
            return _mapper.Map<ImportResourceState.Types.Response>(response);
        }

        public override async Task<Stop.Types.Response> Stop(Stop.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Models.StopProvider.Request>(request);
            var response = await _providerAdapter.StopProvider(mappedRequest);
            return _mapper.Map<Stop.Types.Response>(response);
        }
    }
}
