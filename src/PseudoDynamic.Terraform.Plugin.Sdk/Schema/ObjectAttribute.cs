namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class ObjectAttribute : ComplexAttribute
    {
        public ObjectAttribute() : base(TerraformTypeConstraint.Object)
        {
        }
    }
}
