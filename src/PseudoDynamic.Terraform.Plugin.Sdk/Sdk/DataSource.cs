using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.Services.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class DataSource
    {
        public class ValidateContext<Schema> : ShapingContext
        {
            /// <summary>
            /// Represents the transmitted configuration made by you in the Terraform project. May
            /// contain unknown values. Keep in mind that if you are interested in differentiating
            /// between null and unknown values, you need to use <see cref="ITerraformValue{T}"/>
            /// in your schema.
            /// </summary>
            public Schema Config { get; }

            internal ValidateContext(Reports reports, ITerraformDynamicDecoder dynamicDecoder, Schema config)
                : base(reports, dynamicDecoder) =>
                Config = config;
        }

        public class ReadContext<Schema, ProviderMetaSchema> : ShapingContext
        {
            /// <summary>
            /// The data Terraform is about to read in the plan phase and in the apply phase.
            /// In the plan phase, <see cref="State"/> may contain Terraform unknown.
            /// </summary>
            public Schema State { get; set; }

            public ProviderMetaSchema ProviderMeta { get; }

            internal ReadContext(
                Reports reports,
                ITerraformDynamicDecoder dynamicDecoder,
                Schema state,
                ProviderMetaSchema providerMeta)
                : base(reports, dynamicDecoder)
            {
                State = state;
                ProviderMeta = providerMeta;
            }
        }
    }
}
