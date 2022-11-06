using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class ProviderService : TerraformService<IProvider?>
    {
        internal static ProviderService Unimplemented(BlockDefinition schema) =>
            new(schema);

        public override IProvider Implementation => base.Implementation;

        [MemberNotNull(nameof(Implementation))]
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        public bool HasImplementation => GetImplementation() is not null;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

        /// <summary>
        /// Creates an unimplemented provider service.
        /// </summary>
        /// <param name="schema"></param>
        private ProviderService(BlockDefinition schema)
            : base(schema)
        {
        }

        public ProviderService(BlockDefinition schema, IProvider instance)
            : base(schema, instance)
        {
        }
    }
}
