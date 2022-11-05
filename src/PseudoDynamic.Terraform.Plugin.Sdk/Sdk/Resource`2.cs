using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Prevents inheriting base")]
    public abstract class Resource<Schema, ProviderMetaSchema> : Resource, IResource<Schema, ProviderMetaSchema>, IDesignTimeTerraformService<Schema>.IResource<ProviderMetaSchema>
        where Schema : class
        where ProviderMetaSchema : class
    {
        /// <inheritdoc/>
        public abstract string TypeName { get; }

        /// <inheritdoc/>
        public virtual Task MigrateState(IMigrateStateContext context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task ImportState() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task ReviseState(IReviseStateContext<Schema, ProviderMetaSchema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task ValidateConfig(IValidateConfigContext<Schema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Plan(IPlanContext<Schema, ProviderMetaSchema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Apply(IApplyContext<Schema, ProviderMetaSchema> context) => Task.CompletedTask;
    }
}
