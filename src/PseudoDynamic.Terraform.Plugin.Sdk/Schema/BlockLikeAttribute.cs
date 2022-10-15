namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Derived classes annotates classes or structs being treated as Terraform object, tuple or block.
    /// </summary>
    public abstract class BlockLikeAttribute : ValueAttribute
    {
        internal BlockLikeAttribute(TerraformTypeConstraint typeConstraint) : base(typeConstraint)
        {
        }
    }
}
