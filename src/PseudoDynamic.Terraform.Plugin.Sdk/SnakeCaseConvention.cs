using CaseExtensions;

namespace PseudoDynamic.Terraform.Plugin
{
    internal class SnakeCaseConvention : INameConvention
    {
        public static readonly SnakeCaseConvention Default = new SnakeCaseConvention();

        public string Format(string name) => name.ToSnakeCase();
    }
}
