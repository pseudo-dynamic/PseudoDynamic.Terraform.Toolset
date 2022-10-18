using CaseExtensions;

namespace PseudoDynamic.Terraform.Plugin
{
    internal class KebabCaseConvention : INameConvention
    {
        public static readonly KebabCaseConvention Default = new KebabCaseConvention();

        public string Format(string name) => name.ToKebabCase();
    }
}
