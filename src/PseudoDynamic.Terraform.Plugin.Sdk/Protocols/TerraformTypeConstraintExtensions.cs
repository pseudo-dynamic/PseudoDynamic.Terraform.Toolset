using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    internal static class TerraformTypeConstraintExtensions
    {
        public static string GetBlockAttributeTypeString(this TerraformTypeConstraint typeConstraint) => typeConstraint switch {
            TerraformTypeConstraint.String => "string",
            TerraformTypeConstraint.Number => "number",
            TerraformTypeConstraint.Bool => "bool",
            TerraformTypeConstraint.List => "list",
            TerraformTypeConstraint.Set => "set",
            TerraformTypeConstraint.Map => "map",
            TerraformTypeConstraint.Object => "object",
            TerraformTypeConstraint.Tuple => "tuple",
            TerraformTypeConstraint.Any => "dynamic",
            _ => throw new NotSupportedException($"{typeConstraint} is not convertible to a string and cannot be used to represent a block attribute (nested) type")
        };

        public static string GetBlockAttributeTypeJsonString(this TerraformTypeConstraint typeConstraint) =>
            $"\"{GetBlockAttributeTypeString(typeConstraint)}\"";
    }
}
