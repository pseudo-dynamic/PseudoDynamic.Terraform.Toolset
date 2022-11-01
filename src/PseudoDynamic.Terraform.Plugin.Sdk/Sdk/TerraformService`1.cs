using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Describes a Terraform service, which can be <see cref="ProviderService"/>, <see cref="ResourceService"/>, <see cref="DataSourceService"/>, or <see cref="ProvisionerService"/>.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    internal abstract record TerraformService<TImplementation> : ITerraformService<TImplementation>
        where TImplementation : class
    {
        public BlockDefinition Schema { get; }
        public TImplementation Implementation { get; }
        public TypeAccessor ImplementationTypeAccessor { get; }

        protected TerraformService(BlockDefinition schema, TImplementation implementation)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
            ImplementationTypeAccessor = new TypeAccessor(Implementation.GetType());
        }
    }
}
