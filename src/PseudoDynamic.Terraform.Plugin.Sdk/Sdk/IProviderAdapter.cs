using PseudoDynamic.Terraform.Plugin.Protocols.Consolidated;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal interface IProviderAdapter
    {
        Task<GetProviderSchema.Response> GetProviderSchema(GetProviderSchema.Request request);

        Task<ConfigureProvider.Response> ConfigureProvider(ConfigureProvider.Request request);

        Task<ValidateProviderConfig.Response> ValidateProviderConfig(ValidateProviderConfig.Request request);

        Task<ReadDataSource.Response> ReadDataSource(ReadDataSource.Request request);

        Task<ValidateDataResourceConfig.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Request request);

        Task<ReadResource.Response> ReadResource(ReadResource.Request request);

        Task<ValidateResourceConfig.Response> ValidateResourceConfig(ValidateResourceConfig.Request request);

        Task<UpgradeResourceState.Response> UpgradeResourceState(UpgradeResourceState.Request request);

        Task<PlanResourceChange.Response> PlanResourceChange(PlanResourceChange.Request request);

        Task<ApplyResourceChange.Response> ApplyResourceChange(ApplyResourceChange.Request request);

        Task<ImportResourceState.Response> ImportResourceState(ImportResourceState.Request request);

        Task<StopProvider.Response> StopProvider(StopProvider.Request request);
    }
}
