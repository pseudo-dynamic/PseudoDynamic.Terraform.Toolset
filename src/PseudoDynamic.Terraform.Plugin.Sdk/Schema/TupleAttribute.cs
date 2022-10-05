using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class TupleAttribute : TerraformTypeAttribute
    {
        public TerraformType Type { get; }

        public TupleAttribute()
            : base(TerraformType.Tuple)
        {
        }
    }
}
