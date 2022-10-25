using Grpc.Core;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V6
{
    internal class ProviderAdapter : Provider.ProviderBase
    {
        public override Task<GetProviderSchema.Types.Response> GetProviderSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
        {
            return base.GetProviderSchema(request, context);
        }

        public override Task<ValidateProviderConfig.Types.Response> ValidateProviderConfig(ValidateProviderConfig.Types.Request request, ServerCallContext context)
        {
            return base.ValidateProviderConfig(request, context);
        }

        public override Task<ConfigureProvider.Types.Response> ConfigureProvider(ConfigureProvider.Types.Request request, ServerCallContext context)
        {
            return base.ConfigureProvider(request, context);
        }

        public override Task<ValidateDataResourceConfig.Types.Response> ValidateDataResourceConfig(ValidateDataResourceConfig.Types.Request request, ServerCallContext context)
        {
            return base.ValidateDataResourceConfig(request, context);
        }

        public override Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
        {
            return base.ReadDataSource(request, context);
        }

        public override Task<ValidateResourceConfig.Types.Response> ValidateResourceConfig(ValidateResourceConfig.Types.Request request, ServerCallContext context)
        {
            return base.ValidateResourceConfig(request, context);
        }

        public override Task<ReadResource.Types.Response> ReadResource(ReadResource.Types.Request request, ServerCallContext context)
        {
            return base.ReadResource(request, context);
        }

        public override Task<PlanResourceChange.Types.Response> PlanResourceChange(PlanResourceChange.Types.Request request, ServerCallContext context)
        {
            return base.PlanResourceChange(request, context);
        }

        public override Task<ApplyResourceChange.Types.Response> ApplyResourceChange(ApplyResourceChange.Types.Request request, ServerCallContext context)
        {
            return base.ApplyResourceChange(request, context);
        }

        public override Task<UpgradeResourceState.Types.Response> UpgradeResourceState(UpgradeResourceState.Types.Request request, ServerCallContext context)
        {
            return base.UpgradeResourceState(request, context);
        }

        public override Task<ImportResourceState.Types.Response> ImportResourceState(ImportResourceState.Types.Request request, ServerCallContext context)
        {
            return base.ImportResourceState(request, context);
        }

        public override Task<StopProvider.Types.Response> StopProvider(StopProvider.Types.Request request, ServerCallContext context)
        {
            return base.StopProvider(request, context);
        }
    }
}
