using Grpc.Core;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    public class ProviderAdapter : Provider.ProviderBase
    {
        public override Task<GetProviderSchema.Types.Response> GetSchema(GetProviderSchema.Types.Request request, ServerCallContext context)
        {
            return base.GetSchema(request, context);
        }

        public override Task<Configure.Types.Response> Configure(Configure.Types.Request request, ServerCallContext context)
        {
            return base.Configure(request, context);
        }

        public override Task<PrepareProviderConfig.Types.Response> PrepareProviderConfig(PrepareProviderConfig.Types.Request request, ServerCallContext context)
        {
            return base.PrepareProviderConfig(request, context);
        }

        public override Task<ValidateDataSourceConfig.Types.Response> ValidateDataSourceConfig(ValidateDataSourceConfig.Types.Request request, ServerCallContext context)
        {
            return base.ValidateDataSourceConfig(request, context);
        }

        public override Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request, ServerCallContext context)
        {
            return base.ReadDataSource(request, context);
        }

        public override Task<ValidateResourceTypeConfig.Types.Response> ValidateResourceTypeConfig(ValidateResourceTypeConfig.Types.Request request, ServerCallContext context)
        {
            return base.ValidateResourceTypeConfig(request, context);
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

        public override Task<Stop.Types.Response> Stop(Stop.Types.Request request, ServerCallContext context)
        {
            return base.Stop(request, context);
        }
    }
}
