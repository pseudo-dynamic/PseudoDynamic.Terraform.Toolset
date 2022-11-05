using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Prevents inheriting base")]
    public abstract class Provider<Schema> : Provider, IProvider<Schema>, IDesignTimeTerraformService<Schema>.IProvider
        where Schema : class
    {
        public virtual Task ValidateConfig(IValidateConfigContext<Schema> context) => Task.CompletedTask;

        public virtual Task Configure(IConfigureContext<Schema> context) => Task.CompletedTask;
    }
}
