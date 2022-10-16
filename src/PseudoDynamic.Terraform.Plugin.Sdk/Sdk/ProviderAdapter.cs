using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Protocols.Models;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderAdapter : IProviderAdapter
    {
        private readonly IProvider _provider;

        public ProviderAdapter(IProvider provider) =>
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        public Task<GetProviderSchema.Response> GetProviderSchema(GetProviderSchema.Request request) => throw new NotImplementedException();

        public Task<ValidateProviderConfig.Response> ValidateProviderConfig(ValidateProviderConfig.Request request) => throw new NotImplementedException();

        public Task<ConfigureProvider.Response> ConfigureProvider(ConfigureProvider.Request request) => throw new NotImplementedException();

        #region Data sources

        public Task<ReadDataSource.Response> ReadDataSource(ReadDataSource.Request request) => throw new NotImplementedException();

        public Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Request request) => throw new NotImplementedException();

        #endregion

        #region resources

        public Task<ValidateResourceConfig.Response> ValidateResourceConfig(ValidateResourceConfig.Request request) => throw new NotImplementedException();

        public Task<ReadResource.Response> ReadResource(ReadResource.Request request) => throw new NotImplementedException();

        public Task<PlanResourceChange.Response> PlanResourceChange(PlanResourceChange.Request request) => throw new NotImplementedException();

        public Task<ApplyResourceChange.Response> ApplyResourceChange(ApplyResourceChange.Request request) => throw new NotImplementedException();

        public Task<UpgradeResourceState.Response> UpgradeResourceState(UpgradeResourceState.Request request) => throw new NotImplementedException();

        public Task<ImportResourceState.Response> ImportResourceState(ImportResourceState.Request request) => throw new NotImplementedException();

        #endregion

        public Task<StopProvider.Response> StopProvider(StopProvider.Request request) => throw new NotImplementedException();
    }
}
