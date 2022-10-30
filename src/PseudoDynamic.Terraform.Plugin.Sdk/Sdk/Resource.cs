using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class Resource
    {
        internal static readonly GenericTypeAccessor PlanContextAccessor = new(typeof(PlanContext<>));
        internal static readonly GenericTypeAccessor ValidateContextAccessor = new(typeof(ValidateContext<>));

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

        public class PlanContext<Schema> : ShapingContext
        {
            public Schema Config { get; }
            public Schema Planned { get; }

            internal PlanContext(
                Reports reports,
                ITerraformDynamicDecoder dynamicDecoder,
                Schema config,
                Schema planned)
                : base(reports, dynamicDecoder)
            {
                Config = config;
                Planned = planned;
            }
        }
    }
}
