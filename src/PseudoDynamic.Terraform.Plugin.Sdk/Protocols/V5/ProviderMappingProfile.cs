using AutoMapper;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class ProviderMappingProfile : Profile
    {
        public ProviderMappingProfile()
        {
            CreateMap<DynamicValue, Consolidation.DynamicValue>()
                .ReverseMap();

            CreateMap<Consolidation.AttributePath, AttributePath>();
            CreateMap<Consolidation.AttributePath.Step, AttributePath.Types.Step>();
            CreateMap<Consolidation.AttributePath.Step.SelectorOneOfCase, AttributePath.Types.Step.SelectorOneofCase>();

            CreateMap<RawState, Consolidation.RawState>()
                .ReverseMap();

            CreateMap<Consolidation.Diagnostic, Diagnostic>();
            CreateMap<Consolidation.Diagnostic.DiagnosticSeverity, Diagnostic.Types.Severity>();

            CreateMap<GetProviderSchema.Types.Request, Consolidation.GetProviderSchema.Request>();
            CreateMap<Consolidation.GetProviderSchema.ServerCapabilities, GetProviderSchema.Types.ServerCapabilities>();
            CreateMap<Consolidation.GetProviderSchema.Response, GetProviderSchema.Types.Response>();

            CreateMap<PrepareProviderConfig.Types.Request, Consolidation.ValidateProviderConfig.Request>();
            CreateMap<Consolidation.ValidateProviderConfig.Response, PrepareProviderConfig.Types.Response>();

            CreateMap<ValidateResourceTypeConfig.Types.Request, Consolidation.ValidateResourceConfig.Request>();
            CreateMap<Consolidation.ValidateResourceConfig.Response, ValidateResourceTypeConfig.Types.Response>();

            CreateMap<ValidateDataSourceConfig.Types.Request, Consolidation.ValidateDataResourceConfig.Request>();
            CreateMap<Consolidation.ValidateDataResourceConfig.Response, ValidateDataSourceConfig.Types.Response>();

            CreateMap<UpgradeResourceState.Types.Request, Consolidation.UpgradeResourceState.Request>();
            CreateMap<Consolidation.UpgradeResourceState.Response, UpgradeResourceState.Types.Response>();

            CreateMap<Configure.Types.Request, Consolidation.ConfigureProvider.Request>();
            CreateMap<Consolidation.ConfigureProvider.Response, Configure.Types.Response>();

            CreateMap<ReadResource.Types.Request, Consolidation.ReadResource.Request>();
            CreateMap<Consolidation.ReadResource.Response, ReadResource.Types.Response>();

            CreateMap<PlanResourceChange.Types.Request, Consolidation.PlanResourceChange.Request>();
            CreateMap<Consolidation.PlanResourceChange.Response, PlanResourceChange.Types.Response>();

            CreateMap<ApplyResourceChange.Types.Request, Consolidation.ApplyResourceChange.Request>();
            CreateMap<Consolidation.ApplyResourceChange.Response, ApplyResourceChange.Types.Response>();

            CreateMap<ImportResourceState.Types.Request, Consolidation.ImportResourceState.Request>();
            CreateMap<Consolidation.ImportResourceState.ImportedResource, ImportResourceState.Types.ImportedResource>();
            CreateMap<Consolidation.ImportResourceState.Response, ImportResourceState.Types.Response>();

            CreateMap<ReadDataSource.Types.Request, Consolidation.ReadDataSource.Request>();
            CreateMap<Consolidation.ReadDataSource.Response, ReadDataSource.Types.Response>();

            CreateMap<Stop.Types.Request, Consolidation.StopProvider.Request>();
            CreateMap<Consolidation.StopProvider.Response, Stop.Types.Response>();
        }
    }
}
