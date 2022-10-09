using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.Conventions
{
    internal interface IAttributeNameConvention
    {
        string Format(PropertyInfo attribute);
    }
}
