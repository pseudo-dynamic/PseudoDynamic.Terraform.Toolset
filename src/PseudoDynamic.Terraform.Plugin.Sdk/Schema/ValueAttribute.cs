namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValueAttribute : Attribute
    {
        public TerraformTypeConstraint Type { get; }

        public ValueAttribute(TerraformTypeConstraint type) => Type = type;
    }
}
