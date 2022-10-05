using CaseExtensions;

namespace PseudoDynamic.Terraform.Plugin.Schema.AttributeNaming
{
    public class SnakeCaseAttributeNameConvention : IAttributeNameConvention
    {
        public static readonly SnakeCaseAttributeNameConvention Default = new SnakeCaseAttributeNameConvention();

        public string Format(string attributeName) => attributeName.ToSnakeCase();
    }
}
