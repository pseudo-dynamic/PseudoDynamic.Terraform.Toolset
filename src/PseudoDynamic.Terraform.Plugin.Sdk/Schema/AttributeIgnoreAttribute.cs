namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// If used on a property, the property won't be included as block attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class AttributeIgnoreAttribute : Attribute
    {
    }
}
