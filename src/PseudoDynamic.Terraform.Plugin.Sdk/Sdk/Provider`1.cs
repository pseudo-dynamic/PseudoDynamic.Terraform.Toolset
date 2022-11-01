using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract class Provider<ProviderSchema> : DesignTimeTerraformService, IProvider<ProviderSchema>, IDesignTimeTerraformService<ProviderSchema>.IProvider
        where ProviderSchema : class
    {
        public virtual Task Configure(Provider.ConfigureContext<ProviderSchema> context) => Task.CompletedTask;
    }
}
