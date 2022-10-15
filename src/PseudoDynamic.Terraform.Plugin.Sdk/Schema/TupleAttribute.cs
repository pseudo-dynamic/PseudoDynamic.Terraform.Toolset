namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public sealed class TupleAttribute : BlockLikeAttribute
    {
        public TupleAttribute()
            : base(TerraformTypeConstraint.Tuple)
        {
        }
    }
}
