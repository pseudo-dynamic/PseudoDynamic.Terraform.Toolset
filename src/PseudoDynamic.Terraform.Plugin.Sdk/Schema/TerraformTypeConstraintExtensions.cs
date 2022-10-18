namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal static class TerraformTypeConstraintExtensions
    {
        public static bool IsComplex(this TerraformTypeConstraint terraformType) =>
            terraformType is TerraformTypeConstraint.Object or TerraformTypeConstraint.Tuple or TerraformTypeConstraint.Block;

        public static bool IsMonoRange(this TerraformTypeConstraint terraformType) =>
            terraformType is TerraformTypeConstraint.List or TerraformTypeConstraint.Set;

        public static bool IsRange(this TerraformTypeConstraint terraformType) =>
            terraformType.IsMonoRange() || terraformType == TerraformTypeConstraint.Map;

        public static ValueWrapping? ToValueWrapping(this TerraformTypeConstraint typeConstraint) => typeConstraint switch {
            TerraformTypeConstraint.Any
            or TerraformTypeConstraint.Number
            or TerraformTypeConstraint.String
            or TerraformTypeConstraint.Bool
            or TerraformTypeConstraint.Object
            or TerraformTypeConstraint.Tuple
            or TerraformTypeConstraint.Block => default,
            TerraformTypeConstraint.List => ValueWrapping.List,
            TerraformTypeConstraint.Set => ValueWrapping.Set,
            TerraformTypeConstraint.Map => ValueWrapping.Map,
            _ => throw new InvalidOperationException($"Type constraint \"{typeConstraint}\" cannot be translated to a value of {typeof(ValueWrapping).FullName}")
        };
    }
}
