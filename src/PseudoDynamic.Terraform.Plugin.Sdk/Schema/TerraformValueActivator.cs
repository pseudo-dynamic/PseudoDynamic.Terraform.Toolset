namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public sealed class TerraformValueActivator
    {
        private static readonly GenericTypeAccessor TerraformValueAccessor = new GenericTypeAccessor(typeof(TerraformValue<>));

        internal static object CreateInstance(Type typeArgument, bool isNullable, object? value, bool isNull, bool isUnknown) =>
            TerraformValueAccessor.CreateInstance(typeArgument, new[] { isNullable, value, isNull, isUnknown })
            ?? throw new InvalidOperationException();
    }
}
