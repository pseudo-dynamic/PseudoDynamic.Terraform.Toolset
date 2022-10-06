using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class ObjectAttribute : BlockAttribute
    {
        public ObjectAttribute() : base(TerraformType.Object)
        {
        }
    }
}
