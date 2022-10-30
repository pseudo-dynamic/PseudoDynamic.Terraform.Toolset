using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin
{
    /// <summary>
    /// This attribute should be applied on classes or structs that implement <see cref="ITerraformValue{T}"/>.
    /// It helps to indicate which generic parameter is used for <see cref="ITerraformValue{T}"/>.
    /// If the derived class does not provide the generic parameter for <see cref="ITerraformValue{T}"/> anymore,
    /// then you should not use this attribute. It is important to determine the correct generic parameter for
    /// <see cref="ITerraformValue{T}"/> to fully support <see cref="NullabilityInfoContext"/> capabilities.
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
    internal class TerraformValueTypeAttribute : Attribute
    {
    }
}
