using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class Provider<Schema> : DesignTimeTerraformService, IProvider<Schema>, IDesignTimeTerraformService<Schema>.IProvider
        where Schema : class
    {
        public virtual Task Configure(Provider.IConfigureContext<Schema> context) => Task.CompletedTask;
    }
}
