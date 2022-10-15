namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal static class ValueWrappingExtensions
    {
        public static TerraformTypeConstraint ToTypeConstraint(this ValueWrapping wrapping) => wrapping switch {
            ValueWrapping.List => TerraformTypeConstraint.List,
            ValueWrapping.Set => TerraformTypeConstraint.Set,
            ValueWrapping.Map => TerraformTypeConstraint.Map,
            _ => throw new InvalidOperationException($"Value wrapping \"{wrapping}\" cannot be translated to a value of {typeof(TerraformTypeConstraint).FullName}")
        };
    }
}
