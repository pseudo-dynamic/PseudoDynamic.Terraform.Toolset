namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal interface IProviderAdapter
    {
        Task<Consolidated.GetProviderSchema.Response> GetProviderSchema(Consolidated.GetProviderSchema.Request request);

        Task<Consolidated.ConfigureProvider.Response> ConfigureProvider(Consolidated.ConfigureProvider.Request request);

        Task<Consolidated.ValidateProviderConfig.Response> ValidateProviderConfig(Consolidated.ValidateProviderConfig.Request request);

        Task<Consolidated.ReadDataSource.Response> ReadDataSource(Consolidated.ReadDataSource.Request request);

        Task<Consolidated.ValidateDataResourceConfig.Response> ValidateDataResourceConfig(Consolidated.ValidateDataResourceConfig.Request request);

        Task<Consolidated.ReadResource.Response> ReadResource(Consolidated.ReadResource.Request request);

        Task<Consolidated.ValidateResourceConfig.Response> ValidateResourceConfig(Consolidated.ValidateResourceConfig.Request request);

        Task<Consolidated.UpgradeResourceState.Response> UpgradeResourceState(Consolidated.UpgradeResourceState.Request request);

        Task<Consolidated.PlanResourceChange.Response> PlanResourceChange(Consolidated.PlanResourceChange.Request request);

        Task<Consolidated.ApplyResourceChange.Response> ApplyResourceChange(Consolidated.ApplyResourceChange.Request request);

        Task<Consolidated.ImportResourceState.Response> ImportResourceState(Consolidated.ImportResourceState.Request request);

        Task<Consolidated.StopProvider.Response> StopProvider(Consolidated.StopProvider.Request request);
    }
}
