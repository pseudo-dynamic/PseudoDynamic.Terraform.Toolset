using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.Services.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public sealed class Provider : Provider<object>
    {
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
