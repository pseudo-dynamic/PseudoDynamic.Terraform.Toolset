using System.Runtime.CompilerServices;
using AutoMapper;
using Google.Protobuf.Collections;
using Grpc.Core;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class ProviderAdapter : Provider.ProviderBase
    {
        private static void AddError(
            RepeatedField<Diagnostic> collection,
            Exception error,
            [CallerMemberName] string? caller = null) => collection.Add(new Diagnostic() {
                Severity = Diagnostic.Types.Severity.Error,
                Detail = $"{caller} failed",
                Summary = error.Message
            });

        private readonly IProviderAdapter _providerAdapter;
        private readonly IMapper _mapper;

        public ProviderAdapter(IProviderAdapter providerAdapter, IMapper mapper)
        {
            _providerAdapter = providerAdapter;
            _mapper = mapper;
        }

        public override async Task<GetProviderSchema.Types.Response> GetSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.GetProviderSchema.Request>(request);
                var response = await _providerAdapter.GetProviderSchema(mappedRequest);
                return _mapper.Map<GetProviderSchema.Types.Response>(response);
            } catch (Exception error) {
                var response = new GetProviderSchema.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<Configure.Types.Response> Configure(Configure.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ConfigureProvider.Request>(request);
                var response = await _providerAdapter.ConfigureProvider(mappedRequest);
                return _mapper.Map<Configure.Types.Response>(response);
            } catch (Exception error) {
                var response = new Configure.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<PrepareProviderConfig.Types.Response> PrepareProviderConfig(PrepareProviderConfig.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ValidateProviderConfig.Request>(request);
                var response = await _providerAdapter.ValidateProviderConfig(mappedRequest);
                return _mapper.Map<PrepareProviderConfig.Types.Response>(response);
            } catch (Exception error) {
                var response = new PrepareProviderConfig.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<ValidateDataSourceConfig.Types.Response> ValidateDataSourceConfig(ValidateDataSourceConfig.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ValidateDataResourceConfig.Request>(request);
                var response = await _providerAdapter.ValidateDataResourceConfig(mappedRequest);
                return _mapper.Map<ValidateDataSourceConfig.Types.Response>(response);
            } catch (Exception error) {
                var response = new ValidateDataSourceConfig.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ReadDataSource.Request>(request);
                var response = await _providerAdapter.ReadDataSource(mappedRequest);
                return _mapper.Map<ReadDataSource.Types.Response>(response);
            } catch (Exception error) {
                var response = new ReadDataSource.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<ValidateResourceTypeConfig.Types.Response> ValidateResourceTypeConfig(ValidateResourceTypeConfig.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ValidateResourceConfig.Request>(request);
                var response = await _providerAdapter.ValidateResourceConfig(mappedRequest);
                return _mapper.Map<ValidateResourceTypeConfig.Types.Response>(response);
            } catch (Exception error) {
                var response = new ValidateResourceTypeConfig.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ReadResource.Request>(request);
                var response = await _providerAdapter.ReadResource(mappedRequest);
                return _mapper.Map<ReadResource.Types.Response>(response);
            } catch (Exception error) {
                var response = new ReadResource.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.PlanResourceChange.Request>(request);
                var response = await _providerAdapter.PlanResourceChange(mappedRequest);
                return _mapper.Map<PlanResourceChange.Types.Response>(response);
            } catch (Exception error) {
                var response = new PlanResourceChange.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ApplyResourceChange.Request>(request);
                var response = await _providerAdapter.ApplyResourceChange(mappedRequest);
                return _mapper.Map<ApplyResourceChange.Types.Response>(response);
            } catch (Exception error) {
                var response = new ApplyResourceChange.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.UpgradeResourceState.Request>(request);
                var response = await _providerAdapter.UpgradeResourceState(mappedRequest);
                return _mapper.Map<UpgradeResourceState.Types.Response>(response);
            } catch (Exception error) {
                var response = new UpgradeResourceState.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context)
        {
            try {
                var mappedRequest = _mapper.Map<Consolidation.ImportResourceState.Request>(request);
                var response = await _providerAdapter.ImportResourceState(mappedRequest);
                return _mapper.Map<ImportResourceState.Types.Response>(response);
            } catch (Exception error) {
                var response = new ImportResourceState.Types.Response();
                AddError(response.Diagnostics, error);
                return response;
            }
        }

        public override async Task<Stop.Types.Response> Stop(Stop.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Map<Consolidation.StopProvider.Request>(request);
            var response = await _providerAdapter.StopProvider(mappedRequest);
            return _mapper.Map<Stop.Types.Response>(response);
        }
    }
}
