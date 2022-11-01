using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal class DataSourceServiceFactory : TerraformServiceFactory
    {
        public DataSourceServiceFactory(IServiceProvider serviceProvider, SchemaBuilder schemaBuilder)
            : base(serviceProvider, schemaBuilder)
        {
        }

        public DataSourceService Build(DataSourceServiceDescriptor descriptor) =>
            new DataSourceService(
                BuildSchema(descriptor.SchemaType),
                CreateImplementation<IDataSource>(descriptor));
    }
}
