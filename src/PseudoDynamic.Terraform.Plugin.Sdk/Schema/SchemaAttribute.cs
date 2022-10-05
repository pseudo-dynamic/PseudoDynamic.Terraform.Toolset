namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// By annotating a class with this attribute the class can be used as Terraform schema.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class SchemaAttribute : Attribute
    {
    }
}
