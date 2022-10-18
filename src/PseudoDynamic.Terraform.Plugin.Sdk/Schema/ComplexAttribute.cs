namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Derived classes annotates classes or structs being treated as Terraform object, tuple or block.
    /// </summary>
    public abstract class ComplexAttribute : ValueAttribute
    {
        internal ComplexAttribute(TerraformTypeConstraint typeConstraint) : base(typeConstraint)
        {
        }
    }
}
