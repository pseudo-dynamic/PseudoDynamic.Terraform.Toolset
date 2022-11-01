using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class ProviderService : TerraformService<IProvider?>
    {
        internal static ProviderService Unimplemented(BlockDefinition schema) =>
            new ProviderService(schema);

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
