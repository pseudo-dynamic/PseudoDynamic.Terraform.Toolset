namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    public interface IDesignTimeTerraformService<out Schema>
        where Schema : class
    {
        internal Type SchemaType => typeof(Schema);

        public interface IProvider : IDesignTimeTerraformService<Schema>
        {
        }

        public interface IResource<ProviderSchemaType> : IDesignTimeTerraformService<Schema>
        {
        }

        public interface IDataSource<ProviderSchemaType> : IDesignTimeTerraformService<Schema>
        {
        }

        public interface IProvisioner<ProviderSchemaType> : IDesignTimeTerraformService<Schema>
        {
        }
    }
}
