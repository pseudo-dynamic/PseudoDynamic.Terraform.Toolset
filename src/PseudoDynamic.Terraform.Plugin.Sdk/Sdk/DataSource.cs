using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;
using static PseudoDynamic.Terraform.Plugin.Sdk.TerraformService;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class DataSource
    {
        internal static readonly GenericTypeAccessor ValidateContextAccessor = new(typeof(ValidateContext<>));
        internal static readonly GenericTypeAccessor ReadContextAccessor = new(typeof(ReadContext<>));

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

        public class ReadContext<Schema> : ShapingContext
        {
            public Schema Computed { get; }

            internal ReadContext(
                Reports reports,
                ITerraformDynamicDecoder dynamicDecoder,
                Schema computed)
                : base(reports, dynamicDecoder) =>
                Computed = computed;
        }
    }
}
