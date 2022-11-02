using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class DataSource<Schema, ProviderMetaSchema> : DataSource, IDataSource<Schema, ProviderMetaSchema>, IDesignTimeTerraformService<Schema>.IDataSource<ProviderMetaSchema>
        where Schema : class
        where ProviderMetaSchema : class
    {
        /// <inheritdoc/>
        public abstract string TypeName { get; }

        /// <inheritdoc/>
        public virtual Task ValidateConfig(IValidateConfigContext<Schema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Read(IReadContext<Schema, ProviderMetaSchema> context) => Task.CompletedTask;
    }
}
