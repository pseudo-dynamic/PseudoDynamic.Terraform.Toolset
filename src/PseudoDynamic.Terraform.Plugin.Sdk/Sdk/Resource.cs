using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.Services.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class Resource
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

        public class PlanContext<Schema, ProviderMetaSchema> : ShapingContext
        {
            public Schema Config { get; }
            public Schema Plan { get; set; }
            public ProviderMetaSchema ProviderMeta { get; set; }

            internal PlanContext(
                Reports reports,
                ITerraformDynamicDecoder dynamicDecoder,
                Schema config,
                Schema plan,
                ProviderMetaSchema providerMeta)
                : base(reports, dynamicDecoder)
            {
                Config = config;
                Plan = plan;
                ProviderMeta = providerMeta;
            }
        }
    }
}
