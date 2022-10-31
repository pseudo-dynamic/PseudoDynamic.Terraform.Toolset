namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal static class ValueWrappingExtensions
    {
        public static TerraformTypeConstraint ToTypeConstraint(this ValueDefinitionWrapping wrapping) => wrapping switch {
            ValueDefinitionWrapping.List => TerraformTypeConstraint.List,
            ValueDefinitionWrapping.Set => TerraformTypeConstraint.Set,
            ValueDefinitionWrapping.Map => TerraformTypeConstraint.Map,
            _ => throw new InvalidOperationException($"Value wrapping \"{wrapping}\" cannot be translated to a value of {typeof(TerraformTypeConstraint).FullName}")
        };
    }
}
