using PseudoDynamic.Terraform.Plugin.Sdk.Services;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.Services.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class Resource : DesignTimeTerraformService
    {
        // We do not allow inheritance from the public
        internal Resource()
        {
        }

        public interface IMigrateStateContext
        {
            int Version { get; }
        }

        internal class MigrateStateContext : IBaseContext, IMigrateStateContext
        {
            public Reports Reports { get; }
            public int Version { get; }

            internal MigrateStateContext(Reports reports, int version)
            {
                Reports = reports;
                Version = version;
            }
        }

        public interface IReviseStateContext<Schema, out ProviderMetaSchema> : IBaseContext, IShapingContext, IStateContext<Schema>, IProviderMetaContext<ProviderMetaSchema>
        {
            new Schema State { get; set; }
        }

        internal class ReviseStateContext<Schema, ProviderMetaSchema> : IReviseStateContext<Schema, ProviderMetaSchema>
        {
            public Reports Reports { get; }
            public ITerraformDynamicDecoder DynamicDecoder { get; }
            public Schema State { get; set; }
            public ProviderMetaSchema ProviderMeta { get; }

            internal ReviseStateContext(Reports reports, ITerraformDynamicDecoder dynamicDecoder, Schema state, ProviderMetaSchema providerMeta)
            {
                Reports = reports;
                DynamicDecoder = dynamicDecoder;
                State = state;
                ProviderMeta = providerMeta;
            }
        }

        public interface IValidateConfigContext<out Schema> : IBaseContext, IShapingContext, IConfigContext<Schema>
        {
        }

        internal class ValidateConfigContext<Schema> : IValidateConfigContext<Schema>
        {
            public Reports Reports { get; }
            public ITerraformDynamicDecoder DynamicDecoder { get; }
            public Schema Config { get; }

            internal ValidateConfigContext(Reports reports, ITerraformDynamicDecoder dynamicDecoder, Schema config)
            {
                Reports = reports;
                DynamicDecoder = dynamicDecoder;
                Config = config;
            }
        }

        public interface IPlanContext<Schema>
        {
            /// <summary>
            /// The planned new state for the resource. Terraform 1.3 and later
            /// supports resource destroy planning, in which this will contain a null
            /// value. Is <see langword="null"/> when deleting this resource.
            /// </summary>
            Schema Plan { get; set; }
        }

        public interface IChangeContext<Schema, out ProviderMetaSchema> : IBaseContext, IShapingContext, IConfigContext<Schema>, IPlanContext<Schema>, IProviderMetaContext<ProviderMetaSchema>
        {
        }

        public interface ICreateContext<Schema, out ProviderMetaSchema> : IChangeContext<Schema, ProviderMetaSchema>
        {
        }

        public interface IUpdateContext<Schema, out ProviderMetaSchema> : IChangeContext<Schema, ProviderMetaSchema>, IStateContext<Schema>
        {
        }

        public interface IDeleteContext<Schema, out ProviderMetaSchema> : IBaseContext, IShapingContext, IStateContext<Schema>, IProviderMetaContext<ProviderMetaSchema>
        {
        }

        public interface IPlanContext<Schema, out ProviderMetaSchema> : ICreateContext<Schema, ProviderMetaSchema>, IUpdateContext<Schema, ProviderMetaSchema>, IDeleteContext<Schema, ProviderMetaSchema>
        {
            /// <inheritdoc cref="IConfigContext{Schema}.Config"/>
            new Schema? Config { get; }

            /// <inheritdoc cref="IStateContext{Schema}.State"/>
            new Schema? State { get; }

            /// <inheritdoc cref="IPlanContext{Schema}.Plan"/>
            new Schema? Plan { get; set; }
        }

        public interface IApplyContext<Schema, out ProviderMetaSchema> : IPlanContext<Schema, ProviderMetaSchema>
        {
        }

        internal class PlanContext<Schema, ProviderMetaSchema> : IApplyContext<Schema, ProviderMetaSchema>
        {
            public Reports Reports { get; }
            public ITerraformDynamicDecoder DynamicDecoder { get; }
            public Schema? Config { get; }
            public Schema? State { get; }
            public Schema? Plan { get; set; }

            Schema IConfigContext<Schema>.Config => Config ?? throw new InvalidOperationException();
            Schema IStateContext<Schema>.State => Plan ?? throw new InvalidOperationException();

            Schema IPlanContext<Schema>.Plan {
                get => Plan ?? throw new InvalidOperationException();
                set => Plan = value ?? throw new InvalidOperationException();
            }

            /// <summary>
            /// The metadata from the provider_meta block of the module.
            /// </summary>
            public ProviderMetaSchema ProviderMeta { get; }

            internal PlanContext(
                Reports reports,
                ITerraformDynamicDecoder dynamicDecoder,
                Schema? config,
                Schema? state,
                Schema? plan,
                ProviderMetaSchema providerMeta)
            {
                Reports = reports;
                DynamicDecoder = dynamicDecoder;
                Config = config;
                State = state;
                Plan = plan;
                ProviderMeta = providerMeta;
            }
        }
    }
}
