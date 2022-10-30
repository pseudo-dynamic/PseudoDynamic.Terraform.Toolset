using AutoMapper;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    internal class ProviderMappingProfile : Profile
    {
        public ProviderMappingProfile()
        {
            CreateMap<DynamicValue, Consolidated.DynamicValue>()
                .ReverseMap();

            CreateMap<Consolidated.AttributePath, AttributePath>();
            CreateMap<Consolidated.AttributePath.Step, AttributePath.Types.Step>();
            CreateMap<Consolidated.AttributePath.Step.SelectorOneOfCase, AttributePath.Types.Step.SelectorOneofCase>();

            CreateMap<RawState, Consolidated.RawState>()
                .ReverseMap();

            CreateMap<Consolidated.Diagnostic, Diagnostic>();
            CreateMap<Consolidated.Diagnostic.DiagnosticSeverity, Diagnostic.Types.Severity>();

            CreateMap<GetProviderSchema.Types.Request, Consolidated.GetProviderSchema.Request>();
            CreateMap<Consolidated.GetProviderSchema.ServerCapabilities, GetProviderSchema.Types.ServerCapabilities>();
            CreateMap<Consolidated.GetProviderSchema.Response, GetProviderSchema.Types.Response>();

            CreateMap<PrepareProviderConfig.Types.Request, Consolidated.ValidateProviderConfig.Request>();
            CreateMap<Consolidated.ValidateProviderConfig.Response, PrepareProviderConfig.Types.Response>();

            CreateMap<ValidateResourceTypeConfig.Types.Request, Consolidated.ValidateResourceConfig.Request>();
            CreateMap<Consolidated.ValidateResourceConfig.Response, ValidateResourceTypeConfig.Types.Response>();

            CreateMap<ValidateDataSourceConfig.Types.Request, Consolidated.ValidateDataResourceConfig.Request>();
            CreateMap<Consolidated.ValidateDataResourceConfig.Response, ValidateDataSourceConfig.Types.Response>();

            CreateMap<UpgradeResourceState.Types.Request, Consolidated.UpgradeResourceState.Request>();
            CreateMap<Consolidated.UpgradeResourceState.Response, UpgradeResourceState.Types.Response>();

            CreateMap<Configure.Types.Request, Consolidated.ConfigureProvider.Request>();
            CreateMap<Consolidated.ConfigureProvider.Response, Configure.Types.Response>();

            CreateMap<ReadResource.Types.Request, Consolidated.ReadResource.Request>();
            CreateMap<Consolidated.ReadResource.Response, ReadResource.Types.Response>();

            CreateMap<PlanResourceChange.Types.Request, Consolidated.PlanResourceChange.Request>();
            CreateMap<Consolidated.PlanResourceChange.Response, PlanResourceChange.Types.Response>();

            CreateMap<ApplyResourceChange.Types.Request, Consolidated.ApplyResourceChange.Request>();
            CreateMap<Consolidated.ApplyResourceChange.Response, ApplyResourceChange.Types.Response>();

            CreateMap<ImportResourceState.Types.Request, Consolidated.ImportResourceState.Request>();
            CreateMap<Consolidated.ImportResourceState.ImportedResource, ImportResourceState.Types.ImportedResource>();
            CreateMap<Consolidated.ImportResourceState.Response, ImportResourceState.Types.Response>();

            CreateMap<ReadDataSource.Types.Request, Consolidated.ReadDataSource.Request>();
            CreateMap<Consolidated.ReadDataSource.Response, ReadDataSource.Types.Response>();

            CreateMap<Stop.Types.Request, Consolidated.StopProvider.Request>();
            CreateMap<Consolidated.StopProvider.Response, Stop.Types.Response>();
        }
    }
}
