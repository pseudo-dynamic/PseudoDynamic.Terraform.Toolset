using AutoMapper;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class ProviderMapper : Profile
    {
        public ProviderMapper()
        {
            CreateMap<DynamicValue, Models.DynamicValue>()
                .ReverseMap();

            CreateMap<Models.AttributePath, AttributePath>();
            CreateMap<Models.AttributePath.Step, AttributePath.Types.Step>();
            CreateMap<Models.AttributePath.Step.SelectorOneOfCase, AttributePath.Types.Step.SelectorOneofCase>();

            CreateMap<RawState, Models.RawState>()
                .ReverseMap();

            CreateMap<Models.Diagnostic, Diagnostic>();
            CreateMap<Models.Diagnostic.DiagnosticSeverity, Diagnostic.Types.Severity>();

            CreateMap<GetProviderSchema.Types.Request, Models.GetProviderSchema.Request>();
            CreateMap<Models.GetProviderSchema.ServerCapabilities, GetProviderSchema.Types.ServerCapabilities>();
            CreateMap<Models.GetProviderSchema.Response, GetProviderSchema.Types.Response>();

            CreateMap<PrepareProviderConfig.Types.Request, Models.ValidateProviderConfig.Request>();
            CreateMap<Models.ValidateProviderConfig.Response, PrepareProviderConfig.Types.Response>();

            CreateMap<ValidateResourceTypeConfig.Types.Request, Models.ValidateResourceConfig.Request>();
            CreateMap<Models.ValidateResourceConfig.Response, ValidateResourceTypeConfig.Types.Response>();

            CreateMap<ValidateDataSourceConfig.Types.Request, Models.ValidateDataResourceConfig.Request>();
            CreateMap<Models.ValidateDataResourceConfig.Response, ValidateDataSourceConfig.Types.Response>();

            CreateMap<UpgradeResourceState.Types.Request, Models.UpgradeResourceState.Request>();
            CreateMap<Models.UpgradeResourceState.Response, UpgradeResourceState.Types.Response>();

            CreateMap<Configure.Types.Request, Models.ConfigureProvider.Request>();
            CreateMap<Models.ConfigureProvider.Response, Configure.Types.Response>();

            CreateMap<ReadResource.Types.Request, Models.ReadResource.Request>();
            CreateMap<Models.ReadResource.Response, ReadResource.Types.Response>();

            CreateMap<PlanResourceChange.Types.Request, Models.PlanResourceChange.Request>();
            CreateMap<Models.PlanResourceChange.Response, PlanResourceChange.Types.Response>();

            CreateMap<ApplyResourceChange.Types.Request, Models.ApplyResourceChange.Request>();
            CreateMap<Models.ApplyResourceChange.Response, ApplyResourceChange.Types.Response>();

            CreateMap<ImportResourceState.Types.Request, Models.ImportResourceState.Request>();
            CreateMap<Models.ImportResourceState.ImportedResource, ImportResourceState.Types.ImportedResource>();
            CreateMap<Models.ImportResourceState.Response, ImportResourceState.Types.Response>();

            CreateMap<ReadDataSource.Types.Request, Models.ReadDataSource.Request>();
            CreateMap<Models.ReadDataSource.Response, ReadDataSource.Types.Response>();

            CreateMap<Stop.Types.Request, Models.StopProvider.Request>();
            CreateMap<Models.StopProvider.Response, Stop.Types.Response>();
        }
    }
}
