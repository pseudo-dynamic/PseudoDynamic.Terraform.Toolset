namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IProvider<Schema> where Schema : class
    {
        Task Configure(Provider.ConfigureContext<Schema> context);
    }
}
