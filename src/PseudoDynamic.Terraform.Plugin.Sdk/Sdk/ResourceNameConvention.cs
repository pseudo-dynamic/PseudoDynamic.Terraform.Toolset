namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ResourceNameConvention : IResourceNameConvention
    {
        public string Format(string resourceName, ResourceNameConventionContext context)
        {
            return resourceName;
        }
    }

    public class ResourceNameConventionContext
    {
        public IProvider provider { get; }

        public ResourceNameConventionContext(IProvider provider) =>
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }
}
