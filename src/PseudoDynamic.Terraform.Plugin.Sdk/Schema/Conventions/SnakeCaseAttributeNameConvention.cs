using CaseExtensions;

namespace PseudoDynamic.Terraform.Plugin.Schema.Conventions
{
    public class SnakeCaseConvention : INameConvention
    {
        public static readonly SnakeCaseConvention Default = new SnakeCaseConvention();

        public string Format(string attributeName) => attributeName.ToSnakeCase();
    }
}
