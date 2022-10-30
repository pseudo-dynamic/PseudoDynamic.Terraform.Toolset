using PseudoDynamic.Terraform.Plugin.Sdk.Transcoding;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class DataSourceServiceFactory : TerraformServiceFactory
    {
        public DataSourceServiceFactory(IServiceProvider serviceProvider, DynamicDefinitionResolver dynamicResolver)
            : base(serviceProvider, dynamicResolver)
        {
        }

        public DataSourceService Build(DataSourceServiceDescriptor resourceDescriptor) =>
            new DataSourceService(
                BuildSchema(resourceDescriptor),
                BuildService<INameProvider>(resourceDescriptor));
    }
}
