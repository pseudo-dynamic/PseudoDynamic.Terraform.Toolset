using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// By annotating a class with this attribute the class can be used as Terraform schema or nested block.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BlockAttribute : TerraformTypeAttribute
    {
        internal BlockAttribute(TerraformType type) : base(type)
        {
        }

        public BlockAttribute() : base(TerraformType.Block)
        {
        }
    }
}
