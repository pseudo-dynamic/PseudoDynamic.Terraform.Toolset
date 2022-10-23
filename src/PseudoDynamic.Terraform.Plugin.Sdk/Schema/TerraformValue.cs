namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public static class TerraformValue
    {
        public static TerraformValue<T> OfValue<T>(T value, bool isUnknown = false) =>
            isUnknown ? OfUnknown<T>(isUnknown) : new TerraformValue<T>() { Value = value };

        public static TerraformValue<T> OfNull<T>(bool isNull, bool isUnknown = false) =>
            isUnknown ? OfUnknown<T>(isUnknown) : new TerraformValue<T>() { IsNull = isNull };

        public static TerraformValue<T> OfUnknown<T>(bool isUnknown) =>
            new TerraformValue<T>() { IsUnknown = isUnknown };

        private static readonly GenericTypeAccessor TerraformValueAccessor = new GenericTypeAccessor(typeof(TerraformValue<>));

        internal static object CreateInstance(Type typeArgument, bool isNullable, object? value, bool isNull, bool isUnknown)
        {
            var constructorArguments = new[] { isNullable, value, isNull, isUnknown };

            return TerraformValueAccessor
                .GetTypeAccessor(typeArgument)
                .GetPrivateInstanceConstructor(constructorArguments.Length)
                .Invoke(constructorArguments)
                ?? throw new InvalidOperationException();
        }
    }
}
