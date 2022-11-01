using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class DataSourceService : TerraformService<IDataSource>
    {
        public DataSourceService(BlockDefinition schema, IDataSource dataSource)
            : base(schema, dataSource) =>
            TerraformNameConventionException.EnsureResourceTypeNameConvention(dataSource.Name);
    }
}
