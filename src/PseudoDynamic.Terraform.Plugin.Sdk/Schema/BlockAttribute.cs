namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Annotates a class for being used as Terraform (nested) block.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BlockAttribute : ValueAttribute
    {
        protected BlockAttribute(TerraformTypeConstraint typeConstraint) : base(typeConstraint)
        {
        }

        public BlockAttribute() : base(TerraformTypeConstraint.Block)
        {
        }
    }
}
