namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IResourceNameConvention
    {
        string Format(string resourceName, ResourceNameConventionContext context);
    }
}
