namespace PseudoDynamic.Terraform.Plugin.Types
{
    internal static class TerraformTypeExtensions
    {
        public static bool IsBlockType(this TerraformType terraformType) =>
            terraformType is TerraformType.Object or TerraformType.Tuple or TerraformType.NestedBlock;

        public static bool IsRangeType(this TerraformType terraformType) =>
            terraformType is TerraformType.List or TerraformType.Set or TerraformType.Map;

        public static bool IsReferenceType(this TerraformType terraformType) =>
            terraformType.IsBlockType() || terraformType is TerraformType.Any
            or TerraformType.List or TerraformType.Set or TerraformType.Map
            or TerraformType.Object or TerraformType.NestedBlock;
    }
}
