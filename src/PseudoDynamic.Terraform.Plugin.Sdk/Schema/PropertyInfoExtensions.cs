using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal static class PropertyInfoExtensions
    {
        public static string GetPath(this PropertyInfo property) =>
            $"{property.DeclaringType!.FullName}.{property.Name}";
    }
}
