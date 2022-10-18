namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class ObjectAttribute : ComplexAttribute
    {
        public ObjectAttribute() : base(TerraformTypeConstraint.Object)
        {
        }
    }
}
