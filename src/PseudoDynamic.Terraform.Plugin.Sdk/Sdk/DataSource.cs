using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.Services.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class DataSource
    {
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

        public interface IReadContext<Schema, out ProviderMetaSchema> : IBaseContext, IShapingContext, IStateContext<Schema>, IProviderMetaContext<ProviderMetaSchema>
        {
            /// <summary>
            /// The current state of the resource. Can be <see langword="null"/>,
            /// if you creating this resource.
            /// </summary>
            new Schema State { get; set; }
        }

        internal class ReadContext<Schema, ProviderMetaSchema> : IReadContext<Schema, ProviderMetaSchema>
        {
            public Reports Reports { get; }
            public ITerraformDynamicDecoder DynamicDecoder { get; }
            public Schema State { get; set; }
            public ProviderMetaSchema ProviderMeta { get; }

            internal ReadContext(
                Reports reports,
                ITerraformDynamicDecoder dynamicDecoder,
                Schema state,
                ProviderMetaSchema providerMeta)
            {
                Reports = reports;
                DynamicDecoder = dynamicDecoder;
                State = state;
                ProviderMeta = providerMeta;
            }
        }
    }
}
