namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ObjectAttribute : ComplexAttribute
    {
        [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Prevents inheriting base")]
        public ObjectAttribute() : base(TerraformTypeConstraint.Object)
        {
        }
    }
}
