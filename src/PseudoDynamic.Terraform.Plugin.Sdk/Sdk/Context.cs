using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public static class Context
    {
        /// <summary>
        /// Represents the context of processing, which can be processing a resource, a data source or a provisioner.
        /// </summary>
        public class Basis
        {
            /// <summary>
            /// Stores Terraform reports.
            /// </summary>
            public Reports Reports { get; }

            /// <summary>
            /// Shortcut to <see cref="Task.CompletedTask"/>.
            /// </summary>
            public Task CompletedTask => Task.CompletedTask;

            internal Basis(Reports reports) =>
                Reports = reports;
        }

        public class Shaping : Basis
        {
            public ITerraformDynamicDecoder DynamicDecoder { get; }

            internal Shaping(Reports reports, ITerraformDynamicDecoder dynamicDecoder)
                : base(reports) =>
                DynamicDecoder = dynamicDecoder ?? throw new ArgumentNullException(nameof(dynamicDecoder));
        }
    }
}
