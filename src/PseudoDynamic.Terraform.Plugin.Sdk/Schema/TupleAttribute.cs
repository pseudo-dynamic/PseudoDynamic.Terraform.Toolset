namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public sealed class TupleAttribute : ComplexAttribute
    {
        public TupleAttribute()
            : base(TerraformTypeConstraint.Tuple)
        {
        }
    }
}
