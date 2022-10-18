namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// This attribute helps to indicate the constructor to be used during deserialization.
    /// Note that the names of constructor parameters must match their individual corresponding
    /// property names and property types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class BlockConstructorAttribute : Attribute
    {
    }
}
