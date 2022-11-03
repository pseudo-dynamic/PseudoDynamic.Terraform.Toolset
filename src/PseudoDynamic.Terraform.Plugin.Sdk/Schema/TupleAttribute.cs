namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TupleAttribute : ComplexAttribute
    {
        public TupleAttribute()
            : base(TerraformTypeConstraint.Tuple)
        {
        }
    }
}
