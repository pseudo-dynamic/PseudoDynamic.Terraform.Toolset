namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class Provider<Schema> : DesignTimeTerraformService, IProvider<Schema>, IDesignTimeTerraformService<Schema>.IProvider
        where Schema : class
    {
        public virtual Task Configure(Provider.ConfigureContext<Schema> context) => Task.CompletedTask;
    }
}
