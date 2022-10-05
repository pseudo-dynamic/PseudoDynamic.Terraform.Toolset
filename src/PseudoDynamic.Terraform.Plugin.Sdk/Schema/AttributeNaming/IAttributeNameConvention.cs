namespace PseudoDynamic.Terraform.Plugin.Schema.AttributeNaming
{
    public interface IAttributeNameConvention
    {
        public string Format(string attributeName);
    }
}
