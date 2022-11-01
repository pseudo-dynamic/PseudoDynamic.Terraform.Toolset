using System.Runtime.CompilerServices;
using AutoMapper;
using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Sdk;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class ProviderAdapter : Provider.ProviderBase
    {
        private static void AddError(
            RepeatedField<Diagnostic> collection,
            Exception error,
            [CallerMemberName] string? caller = null) => collection.Add(new Diagnostic() {
                Severity = Diagnostic.Types.Severity.Error,
                Detail = error.ToString(),
                Summary = $"[.NET/C#] An exception occured during the gRPC call of \"{caller}\""
            });

        private readonly Task<IProviderAdapter> _providerAdapter;
        private readonly Task<IMapper> _mapper;

        public ProviderAdapter(IServiceProvider serviceProvider)
        {
            // We must defer for getting exception message
            _providerAdapter = Task.Run(serviceProvider.GetRequiredService<IProviderAdapter>);
            _mapper = Task.Run(serviceProvider.GetRequiredService<IMapper>);
        }

        private async Task<TResponse> TryCatch<TResponse>(
            Func<Task<TResponse>> processRequest,
            Func<TResponse, RepeatedField<Diagnostic>>? getDiagnostics = null,
            [CallerMemberName] string? caller = null)
            where TResponse : new()
        {
            try {
                return await processRequest();
            } catch (Exception error) {
                var response = new TResponse();

                if (getDiagnostics != null) {
                    AddError(getDiagnostics(response), error, caller);
                }

                return response;
            }
        }

        public override Task<GetProviderSchema.Types.Response> GetSchema(GetProviderSchema.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.GetProviderSchema.Request>(request);
                var response = await _providerAdapter.Result.GetProviderSchema(mappedRequest);
                return _mapper.Result.Map<GetProviderSchema.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<Configure.Types.Response> Configure(Configure.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ConfigureProvider.Request>(request);
                var response = await _providerAdapter.Result.ConfigureProvider(mappedRequest);
                return _mapper.Result.Map<Configure.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<PrepareProviderConfig.Types.Response> PrepareProviderConfig(PrepareProviderConfig.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ValidateProviderConfig.Request>(request);
                var response = await _providerAdapter.Result.ValidateProviderConfig(mappedRequest);
                return _mapper.Result.Map<PrepareProviderConfig.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ValidateDataSourceConfig.Types.Response> ValidateDataSourceConfig(ValidateDataSourceConfig.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ValidateDataResourceConfig.Request>(request);
                var response = await _providerAdapter.Result.ValidateDataResourceConfig(mappedRequest);
                return _mapper.Result.Map<ValidateDataSourceConfig.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ReadDataSource.Request>(request);
                var response = await _providerAdapter.Result.ReadDataSource(mappedRequest);
                return _mapper.Result.Map<ReadDataSource.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ValidateResourceTypeConfig.Types.Response> ValidateResourceTypeConfig(ValidateResourceTypeConfig.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ValidateResourceConfig.Request>(request);
                var response = await _providerAdapter.Result.ValidateResourceConfig(mappedRequest);
                return _mapper.Result.Map<ValidateResourceTypeConfig.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ReadResource.Request>(request);
                var response = await _providerAdapter.Result.ReadResource(mappedRequest);
                return _mapper.Result.Map<ReadResource.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.PlanResourceChange.Request>(request);
                var response = await _providerAdapter.Result.PlanResourceChange(mappedRequest);
                return _mapper.Result.Map<PlanResourceChange.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ApplyResourceChange.Request>(request);
                var response = await _providerAdapter.Result.ApplyResourceChange(mappedRequest);
                return _mapper.Result.Map<ApplyResourceChange.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.UpgradeResourceState.Request>(request);
                var response = await _providerAdapter.Result.UpgradeResourceState(mappedRequest);
                return _mapper.Result.Map<UpgradeResourceState.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                var mappedRequest = _mapper.Result.Map<Consolidated.ImportResourceState.Request>(request);
                var response = await _providerAdapter.Result.ImportResourceState(mappedRequest);
                return _mapper.Result.Map<ImportResourceState.Types.Response>(response);
            }, response => response.Diagnostics);

        public override async Task<Stop.Types.Response> Stop(Stop.Types.Request request, ServerCallContext context)
        {
            var mappedRequest = _mapper.Result.Map<Consolidated.StopProvider.Request>(request);
            var response = await _providerAdapter.Result.StopProvider(mappedRequest);
            return _mapper.Result.Map<Stop.Types.Response>(response);
        }
    }
}
