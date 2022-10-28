using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class ValidateConfig
    {
        internal static readonly GenericTypeAccessor ContextAccessor = new(typeof(Context<>));

        public class Context<Schema> : Context.Shaping
        {
            /// <summary>
            /// Represents the transmitted configuration made by you in the Terraform project. May
            /// contain unknown values. Keep in mind that if you are interested in differentiating
            /// between null and unknown values, you need to use <see cref="ITerraformValue{T}"/>
            /// in your schema.
            /// </summary>
            public Schema Config { get; }

            internal Context(Schema config, Reports reports, ITerraformDynamicDecoder dynamicDecoder)
                : base(reports, dynamicDecoder) =>
                Config = config;
        }
    }
}
