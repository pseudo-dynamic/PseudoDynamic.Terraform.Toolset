using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TerraformTypeAttribute : Attribute
    {
        public TerraformType Type { get; }

        public TerraformTypeAttribute(TerraformType type) => Type = type;
    }
}
