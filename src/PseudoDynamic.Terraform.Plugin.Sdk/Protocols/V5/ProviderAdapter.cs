﻿using System.Runtime.CompilerServices;
using AutoMapper;
using Google.Protobuf.Collections;
using Grpc.Core;
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

        private readonly IProviderAdapter _providerAdapter;
        private readonly IMapper _mapper;

        public ProviderAdapter(IProviderAdapter providerAdapter, IMapper mapper)
        {
            _providerAdapter = providerAdapter ?? throw new ArgumentNullException(nameof(providerAdapter));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                TResponse response = new();

                if (getDiagnostics != null) {
                    AddError(getDiagnostics(response), error, caller);
                }

                return response;
            }
        }

        public override Task<GetProviderSchema.Types.Response> GetSchema(GetProviderSchema.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.GetProviderSchema.Request mappedRequest = _mapper.Map<Consolidated.GetProviderSchema.Request>(request);
                Consolidated.GetProviderSchema.Response response = await _providerAdapter.GetProviderSchema(mappedRequest);
                return _mapper.Map<GetProviderSchema.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<Configure.Types.Response> Configure(Configure.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ConfigureProvider.Request mappedRequest = _mapper.Map<Consolidated.ConfigureProvider.Request>(request);
                Consolidated.ConfigureProvider.Response response = await _providerAdapter.ConfigureProvider(mappedRequest);
                return _mapper.Map<Configure.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<PrepareProviderConfig.Types.Response> PrepareProviderConfig(PrepareProviderConfig.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ValidateProviderConfig.Request mappedRequest = _mapper.Map<Consolidated.ValidateProviderConfig.Request>(request);
                Consolidated.ValidateProviderConfig.Response response = await _providerAdapter.ValidateProviderConfig(mappedRequest);
                return _mapper.Map<PrepareProviderConfig.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ValidateDataSourceConfig.Types.Response> ValidateDataSourceConfig(ValidateDataSourceConfig.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ValidateDataResourceConfig.Request mappedRequest = _mapper.Map<Consolidated.ValidateDataResourceConfig.Request>(request);
                Consolidated.ValidateDataResourceConfig.Response response = await _providerAdapter.ValidateDataResourceConfig(mappedRequest);
                return _mapper.Map<ValidateDataSourceConfig.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ReadDataSource.Request mappedRequest = _mapper.Map<Consolidated.ReadDataSource.Request>(request);
                Consolidated.ReadDataSource.Response response = await _providerAdapter.ReadDataSource(mappedRequest);
                return _mapper.Map<ReadDataSource.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ValidateResourceTypeConfig.Types.Response> ValidateResourceTypeConfig(ValidateResourceTypeConfig.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ValidateResourceConfig.Request mappedRequest = _mapper.Map<Consolidated.ValidateResourceConfig.Request>(request);
                Consolidated.ValidateResourceConfig.Response response = await _providerAdapter.ValidateResourceConfig(mappedRequest);
                return _mapper.Map<ValidateResourceTypeConfig.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ReadResource.Request mappedRequest = _mapper.Map<Consolidated.ReadResource.Request>(request);
                Consolidated.ReadResource.Response response = await _providerAdapter.ReadResource(mappedRequest);
                return _mapper.Map<ReadResource.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.PlanResourceChange.Request mappedRequest = _mapper.Map<Consolidated.PlanResourceChange.Request>(request);
                Consolidated.PlanResourceChange.Response response = await _providerAdapter.PlanResourceChange(mappedRequest);
                return _mapper.Map<PlanResourceChange.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ApplyResourceChange.Request mappedRequest = _mapper.Map<Consolidated.ApplyResourceChange.Request>(request);
                Consolidated.ApplyResourceChange.Response response = await _providerAdapter.ApplyResourceChange(mappedRequest);
                return _mapper.Map<ApplyResourceChange.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.UpgradeResourceState.Request mappedRequest = _mapper.Map<Consolidated.UpgradeResourceState.Request>(request);
                Consolidated.UpgradeResourceState.Response response = await _providerAdapter.UpgradeResourceState(mappedRequest);
                return _mapper.Map<UpgradeResourceState.Types.Response>(response);
            }, response => response.Diagnostics);

        public override Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context) =>
            TryCatch(async () => {
                Consolidated.ImportResourceState.Request mappedRequest = _mapper.Map<Consolidated.ImportResourceState.Request>(request);
                Consolidated.ImportResourceState.Response response = await _providerAdapter.ImportResourceState(mappedRequest);
                return _mapper.Map<ImportResourceState.Types.Response>(response);
            }, response => response.Diagnostics);

        public override async Task<Stop.Types.Response> Stop(Stop.Types.Request request, ServerCallContext context)
        {
            Consolidated.StopProvider.Request mappedRequest = _mapper.Map<Consolidated.StopProvider.Request>(request);
            Consolidated.StopProvider.Response response = await _providerAdapter.StopProvider(mappedRequest);
            return _mapper.Map<Stop.Types.Response>(response);
        }
    }
}
