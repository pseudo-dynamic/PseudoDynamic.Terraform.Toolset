using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Prevents inheriting base")]
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
