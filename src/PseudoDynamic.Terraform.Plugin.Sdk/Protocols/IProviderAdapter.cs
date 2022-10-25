namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal interface IProviderAdapter
    {
        Task<Consolidation.GetProviderSchema.Response> GetProviderSchema(Consolidation.GetProviderSchema.Request request);

        Task<Consolidation.ConfigureProvider.Response> ConfigureProvider(Consolidation.ConfigureProvider.Request request);

        Task<Consolidation.ValidateProviderConfig.Response> ValidateProviderConfig(Consolidation.ValidateProviderConfig.Request request);

        Task<Consolidation.ReadDataSource.Response> ReadDataSource(Consolidation.ReadDataSource.Request request);

        Task<Consolidation.ValidateDataResourceConfig.Response> ValidateDataResourceConfig(Consolidation.ValidateDataResourceConfig.Request request);

        Task<Consolidation.ReadResource.Response> ReadResource(Consolidation.ReadResource.Request request);

        Task<Consolidation.ValidateResourceConfig.Response> ValidateResourceConfig(Consolidation.ValidateResourceConfig.Request request);

        Task<Consolidation.UpgradeResourceState.Response> UpgradeResourceState(Consolidation.UpgradeResourceState.Request request);

        Task<Consolidation.PlanResourceChange.Response> PlanResourceChange(Consolidation.PlanResourceChange.Request request);

        Task<Consolidation.ApplyResourceChange.Response> ApplyResourceChange(Consolidation.ApplyResourceChange.Request request);

        Task<Consolidation.ImportResourceState.Response> ImportResourceState(Consolidation.ImportResourceState.Request request);

        Task<Consolidation.StopProvider.Response> StopProvider(Consolidation.StopProvider.Request request);
    }
}
