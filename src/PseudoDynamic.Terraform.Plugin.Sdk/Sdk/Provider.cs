using PseudoDynamic.Terraform.Plugin.Sdk.Services;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.Services.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class Provider : DesignTimeTerraformService
    {
        // We do not allow inheritance from the public
        internal Provider()
        {
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

        public interface IConfigureContext<out Schema> : IBaseContext, IShapingContext, IConfigContext<Schema>
        {
        }

        internal class ConfigureContext<Schema> : IConfigureContext<Schema>
        {
            public Reports Reports { get; }
            public ITerraformDynamicDecoder DynamicDecoder { get; }
            public Schema Config { get; }

            internal ConfigureContext(Reports reports, ITerraformDynamicDecoder dynamicDecoder, Schema config)
            {
                Reports = reports;
                DynamicDecoder = dynamicDecoder;
                Config = config;
            }
        }
    }
}
