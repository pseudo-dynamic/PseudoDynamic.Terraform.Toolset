using PseudoDynamic.Terraform.Plugin.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Provides helper functions for <see cref="TerraformValue{T}"/>.
    /// </summary>
    public static class TerraformValue
    {
        private static readonly GenericTypeAccessor TerraformValueAccessor = new GenericTypeAccessor(typeof(TerraformValue<>));

        internal static object CreateInstance(Type typeArgument, bool isNullable, object? value, bool isNull, bool isUnknown)
        {
            var constructorArguments = new[] { isNullable, value, isNull, isUnknown };

            return TerraformValueAccessor
                .MakeGenericTypeAccessor(typeArgument)
                .GetPrivateInstanceConstructor(constructorArguments.Length)
                .Invoke(constructorArguments)
                ?? throw new InvalidOperationException();
        }

        /// <summary>
        /// Creates a Terraform value of <paramref name="value"/>, that can result into a Terraform null value if <paramref name="value"/> is <see langword="null"/>.
        /// If <paramref name="isUnknown"/> is true, then the result will be a Terraform unknown value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="isUnknown"></param>
        /// <returns>A non-null instance.</returns>
        public static TerraformValue<T> OfValue<T>(T value, bool isUnknown) =>
            isUnknown ? TerraformValue<T>.Unknown : new TerraformValue<T>(value);

        /// <summary>
        /// Creates a Terraform value of <paramref name="value"/>, that can result into a Terraform null value if <paramref name="value"/> is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>A non-null instance.</returns>
        public static TerraformValue<T> OfValue<T>(T value) =>
            new TerraformValue<T>(value);
    }
}
