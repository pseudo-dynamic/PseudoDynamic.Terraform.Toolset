using CaseExtensions;

namespace PseudoDynamic.Terraform.Plugin.Conventions
{
    internal class KebabCaseConvention : INameConvention
    {
        public static readonly KebabCaseConvention Default = new();

        public string Format(string name) => name.ToKebabCase();
    }
}
