namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ResourceNameConvention : IResourceNameConvention
    {
        public string Format(string resourceName, ResourceNameContext context)
        {
            return resourceName;
        }
    }
}
