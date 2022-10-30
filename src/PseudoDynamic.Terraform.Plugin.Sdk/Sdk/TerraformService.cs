using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class TerraformService
    {
        /// <summary>
        /// Represents the base context for the provider, a resource, a data source or a provisioner.
        /// </summary>
        public class BaseContext
        {
            /// <summary>
            /// Stores Terraform reports.
            /// </summary>
            public Reports Reports { get; }

            internal BaseContext(Reports reports) =>
                Reports = reports;

            /// <summary>
            /// Converts this instance to a completed task.
            /// </summary>
            /// <param name="_"></param>
            public static implicit operator Task(BaseContext _) =>
                Task.CompletedTask;
        }

        public class ShapingContext : BaseContext
        {
            public ITerraformDynamicDecoder DynamicDecoder { get; }

            internal ShapingContext(Reports reports, ITerraformDynamicDecoder dynamicDecoder)
                : base(reports) =>
                DynamicDecoder = dynamicDecoder ?? throw new ArgumentNullException(nameof(dynamicDecoder));
        }
    }
}
