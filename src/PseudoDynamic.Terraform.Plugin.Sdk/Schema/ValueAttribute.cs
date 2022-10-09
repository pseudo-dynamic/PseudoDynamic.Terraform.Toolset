namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValueAttribute : Attribute
    {
        public TerraformTypeConstraint TypeConstraint { get; }

        public ValueAttribute(TerraformTypeConstraint typeConstraint) => TypeConstraint = typeConstraint;
    }
}
