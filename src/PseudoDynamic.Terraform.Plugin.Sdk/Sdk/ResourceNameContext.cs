namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class ResourceNameContext
    {
        public IProvider provider { get; }

        public ResourceNameContext(IProvider provider) =>
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }
}
