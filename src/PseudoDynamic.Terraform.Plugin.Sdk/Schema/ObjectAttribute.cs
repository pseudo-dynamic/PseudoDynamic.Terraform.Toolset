namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class ObjectAttribute : BlockLikeAttribute
    {
        public ObjectAttribute() : base(TerraformTypeConstraint.Object)
        {
        }
    }
}
