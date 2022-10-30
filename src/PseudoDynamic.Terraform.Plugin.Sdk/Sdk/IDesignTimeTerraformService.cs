namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IDesignTimeTerraformService<out Schema> where Schema : class
    {
        internal Type SchemaType => typeof(Schema);

        public interface IProvider : IDesignTimeTerraformService<Schema>
        {
        }

        public interface IResource : IDesignTimeTerraformService<Schema>
        {
        }

        public interface IDataSource : IDesignTimeTerraformService<Schema>
        {
        }

        public interface IProvisioner : IDesignTimeTerraformService<Schema>
        {
        }
    }
}
