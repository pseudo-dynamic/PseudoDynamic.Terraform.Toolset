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

        public static ValueDefinitionWrapping? ToValueWrapping(this TerraformTypeConstraint typeConstraint) => typeConstraint switch {
            TerraformTypeConstraint.Dynamic
            or TerraformTypeConstraint.Number
            or TerraformTypeConstraint.String
            or TerraformTypeConstraint.Bool
            or TerraformTypeConstraint.Object
            or TerraformTypeConstraint.Tuple
            or TerraformTypeConstraint.Block => null,
            TerraformTypeConstraint.List => ValueDefinitionWrapping.List,
            TerraformTypeConstraint.Set => ValueDefinitionWrapping.Set,
            TerraformTypeConstraint.Map => ValueDefinitionWrapping.Map,
            _ => throw new InvalidOperationException($"Type constraint \"{typeConstraint}\" cannot be translated to a value of {typeof(ValueDefinitionWrapping).FullName}")
        };
    }
}
