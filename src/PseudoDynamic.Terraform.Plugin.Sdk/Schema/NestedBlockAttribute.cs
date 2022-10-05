using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class NestedBlockAttribute : TerraformTypeAttribute
    {
        public TerraformType Type { get; }

        public NestedBlockAttribute()
            : base(TerraformType.NestedBlock)
        {
        }
    }
}
