using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// Describes a Terraform service, which can be a provider, a resource, a data source, or a provisioner.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    internal abstract record TerraformService<TService> : ITerraformService<TService>
        where TService : class
    {
        public BlockDefinition Schema { get; }
        public TService Service { get; }
        public TypeAccessor ServiceTypeAccessor { get; }

        protected TerraformService(BlockDefinition schema, TService service)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Service = service ?? throw new ArgumentNullException(nameof(service));
            ServiceTypeAccessor = new TypeAccessor(Service.GetType());
        }
    }
}
