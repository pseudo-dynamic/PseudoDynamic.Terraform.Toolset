using static PseudoDynamic.Terraform.Plugin.Sdk.Resource;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class Resource<Schema> : DesignTimeTerraformService, IResource<Schema>, IDesignTimeTerraformService<Schema>.IResource
        where Schema : class
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
        public virtual Task Plan(PlanContext<Schema> context) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Create() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Update() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task Delete() => Task.CompletedTask;
    }
}
