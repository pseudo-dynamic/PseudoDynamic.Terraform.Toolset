namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DescriptionKindAttribute : Attribute
    {
        public DescriptionKind DescriptionKind { get; }

        public DescriptionKindAttribute(DescriptionKind descriptionKind) =>
            DescriptionKind = descriptionKind;
    }
}
