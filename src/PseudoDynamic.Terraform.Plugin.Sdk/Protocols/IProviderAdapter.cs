namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal interface IProviderAdapter
    {
        Task<Models.ApplyResourceChange.Response> ApplyResourceChange(Models.ApplyResourceChange.Request request);
        Task<Models.ConfigureProvider.Response> ConfigureProvider(Models.ConfigureProvider.Request request);
        Task<Models.GetProviderSchema.Response> GetProviderSchema(Models.GetProviderSchema.Request request);
        Task<Models.ImportResourceState.Response> ImportResourceState(Models.ImportResourceState.Request request);
        Task<Models.PlanResourceChange.Response> PlanResourceChange(Models.PlanResourceChange.Request request);
        Task<Models.ReadDataSource.Response> ReadDataSource(Models.ReadDataSource.Request request);
        Task<Models.ReadResource.Response> ReadResource(Models.ReadResource.Request request);
        Task<Models.StopProvider.Response> StopProvider(Models.StopProvider.Request request);
        Task<Models.UpgradeResourceState.Response> UpgradeResourceState(Models.UpgradeResourceState.Request request);
        Task<Models.ValidateDataResourceConfig.Response> ValidateDataResourceConfig(Models.ValidateDataResourceConfig.Request request);
        Task<Models.ValidateProviderConfig.Response> ValidateProviderConfig(Models.ValidateProviderConfig.Request request);
        Task<Models.ValidateResourceConfig.Response> ValidateResourceConfig(Models.ValidateResourceConfig.Request request);
    }
}
