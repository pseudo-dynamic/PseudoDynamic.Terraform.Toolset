namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class TupleAttribute : BlockAttribute
    {
        public TupleAttribute()
            : base(TerraformTypeConstraint.Tuple)
        {
        }
    }
}
