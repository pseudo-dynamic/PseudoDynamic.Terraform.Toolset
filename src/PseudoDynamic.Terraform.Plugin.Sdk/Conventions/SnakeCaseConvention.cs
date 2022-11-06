using CaseExtensions;

namespace PseudoDynamic.Terraform.Plugin.Conventions
{
    internal class SnakeCaseConvention : INameConvention
    {
        public static readonly SnakeCaseConvention Default = new();

        public string Format(string name) => name.ToSnakeCase();
    }
}
