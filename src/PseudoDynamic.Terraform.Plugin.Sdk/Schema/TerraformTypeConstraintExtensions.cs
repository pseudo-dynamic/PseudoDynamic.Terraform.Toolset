namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal static class TerraformTypeConstraintExtensions
    {
        public static bool IsBlockType(this TerraformTypeConstraint terraformType) =>
            terraformType is TerraformTypeConstraint.Object or TerraformTypeConstraint.Tuple or TerraformTypeConstraint.Block;

        public static bool IsMonoRangeType(this TerraformTypeConstraint terraformType) =>
            terraformType is TerraformTypeConstraint.List or TerraformTypeConstraint.Set;

        public static bool IsRangeType(this TerraformTypeConstraint terraformType) =>
            terraformType.IsMonoRangeType() || terraformType == TerraformTypeConstraint.Map;

        //public static bool IsReferenceType(this TerraformTypeConstraint terraformType) =>
        //    terraformType.IsBlockType() || terraformType is TerraformTypeConstraint.Any
        //    or TerraformTypeConstraint.List or TerraformTypeConstraint.Set or TerraformTypeConstraint.Map
        //    or TerraformTypeConstraint.Object or TerraformTypeConstraint.Block;
    }
}
