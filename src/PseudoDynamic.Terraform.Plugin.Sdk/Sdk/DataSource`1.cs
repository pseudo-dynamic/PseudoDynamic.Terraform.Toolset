namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class DataSource<Schema> : DesignTimeTerraformService, IDataSource<Schema>, IDesignTimeTerraformService<Schema>.IDataSource
        where Schema : class
    {
        /// <inheritdoc/>
        public abstract string TypeName { get; }

        /// <inheritdoc/>
        public virtual Task ValidateConfig(DataSource.ValidateContext<Schema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Read(DataSource.ReadContext<Schema> context) => Task.CompletedTask;
    }
}
