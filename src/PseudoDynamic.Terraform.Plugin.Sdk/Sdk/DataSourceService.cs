using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class DataSourceService : TerraformService<INameProvider>
    {
        public DataSourceService(BlockDefinition schema, INameProvider dataSource)
            : base(schema, dataSource) =>
            TerraformNameConventionException.EnsureResourceTypeNameConvention(dataSource.Name);
    }
}
