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
            /// <summary>
            /// This configuration may contain unknown values if a user uses
            /// interpolation or other functionality that would prevent Terraform
            /// from knowing the value at request time.
            /// </summary>
            public Schema Config { get; }

            /// <summary>
            /// The current state of the resource. Can be <see langword="null"/>,
            /// if you creating this resource.
            /// </summary>
            public Schema? State { get; }

            /// <summary>
            /// The planned new state for the resource. Terraform 1.3 and later
            /// supports resource destroy planning, in which this will contain a null
            /// value.
            /// </summary>
            public Schema? Plan { get; set; }

            /// <summary>
            /// The metadata from the provider_meta block of the module.
            /// </summary>
            public ProviderMetaSchema ProviderMeta { get; set; }

            internal PlanContext(
                Reports reports,
                ITerraformDynamicDecoder dynamicDecoder,
                Schema config,
                Schema? state,
                Schema? plan,
                ProviderMetaSchema providerMeta)
                : base(reports, dynamicDecoder)
            {
                Config = config;
                State = state;
                Plan = plan;
                ProviderMeta = providerMeta;
            }
        }
    }
}
