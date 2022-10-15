namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Annotates a class for being used as Terraform (nested) block.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class BlockAttribute : BlockLikeAttribute
    {
        public BlockAttribute() : base(TerraformTypeConstraint.Block)
        {
        }
    }
}
