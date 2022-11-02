using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class Provider<Schema> : Provider, IProvider<Schema>, IDesignTimeTerraformService<Schema>.IProvider
        where Schema : class
    {
        public virtual Task Configure(IConfigureContext<Schema> context) => Task.CompletedTask;
    }
}
