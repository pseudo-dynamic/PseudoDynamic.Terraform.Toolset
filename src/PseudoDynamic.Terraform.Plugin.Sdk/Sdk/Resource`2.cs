using PseudoDynamic.Terraform.Plugin.Sdk.Services;
using static PseudoDynamic.Terraform.Plugin.Sdk.Resource;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class Resource<Schema, ProviderMetaSchema> : DesignTimeTerraformService, IResource<Schema, ProviderMetaSchema>, IDesignTimeTerraformService<Schema>.IResource<ProviderMetaSchema>
        where Schema : class
        where ProviderMetaSchema : class
    {
        /// <inheritdoc/>
        public abstract string TypeName { get; }

        /// <inheritdoc/>
        public virtual Task MigrateState() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task ImportState() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task ReviseState() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task ValidateConfig(ValidateContext<Schema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Plan(PlanContext<Schema, ProviderMetaSchema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Create() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Update() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Delete() => Task.CompletedTask;
    }
}
