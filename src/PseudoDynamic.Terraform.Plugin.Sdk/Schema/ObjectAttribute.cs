namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class ObjectAttribute : BlockAttribute
    {
        public ObjectAttribute() : base(TerraformTypeConstraint.Object)
        {
        }
    }
}
