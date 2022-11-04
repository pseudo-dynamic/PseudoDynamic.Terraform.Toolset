using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    public static class TerraformService
    {
        public interface IBaseContext
        {
            /// <summary>
            /// Stores Terraform reports.
            /// </summary>
            Reports Reports { get; }

            /// <summary>
            /// Gets a task that is already completed. Shortcut for <see cref="Task.CompletedTask"/>.
            /// </summary>
            Task CompletedTask => Task.CompletedTask;
        }

        public interface IShapingContext
        {
            ITerraformDynamicDecoder DynamicDecoder { get; }
        }

        public interface IStateContext<out Schema>
        {
            /// <summary>
            /// The current state of the resource. Is <see langword="null"/> when creating this resource.
            /// </summary>
            Schema State { get; }
        }

        public interface IConfigContext<out Schema> : IShapingContext
        {
            /// <summary>
            /// Represents the transmitted configuration made by you in the Terraform project. May
            /// contain unknown values. Keep in mind that if you are interested in differentiating
            /// between null and unknown values, you need to use <see cref="ITerraformValue{T}"/>
            /// in your schema. Is <see langword="null"/> when deleting this resource.
            /// </summary>
            Schema Config { get; }
        }

        public interface IProviderMetaContext<out ProviderMetaSchema> : IShapingContext
        {
            /// <summary>
            /// The metadata from the provider_meta block of the module.
            /// </summary>
            ProviderMetaSchema ProviderMeta { get; }
        }
    }
}
