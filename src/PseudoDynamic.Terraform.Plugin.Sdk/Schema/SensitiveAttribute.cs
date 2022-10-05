namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SensitiveAttribute : Attribute
    {
    }
}
