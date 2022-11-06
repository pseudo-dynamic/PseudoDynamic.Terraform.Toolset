using System.Runtime.CompilerServices;
using PseudoDynamic.Terraform.Plugin.Reflection;

namespace PseudoDynamic.Terraform.Plugin
{
    /// <summary>
    /// Provides helper functions for <see cref="TerraformValue{T}"/>.
    /// </summary>
    public static class TerraformValue
    {
        internal static readonly Type ClassGenericTypeDefinition = typeof(TerraformValue<>);
        internal static readonly Type InterfaceGenericTypeDefinition = typeof(ITerraformValue<>);
        internal static readonly Type BaseInterfaceType = typeof(ITerraformValue);

        private static readonly GenericTypeAccessor TerraformValueAccessor = new(ClassGenericTypeDefinition);

        internal static object CreateInstance(Type typeArgument, bool isNullable, object? value, bool isNull, bool isUnknown)
        {
            object?[] constructorArguments = new[] { isNullable, value, isNull, isUnknown };

            return TerraformValueAccessor
                .MakeGenericTypeAccessor(typeArgument)
                .GetPrivateInstanceConstructor(constructorArguments.Length)
                .Invoke(constructorArguments);
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
            new(value);

        /// <summary>
        /// Representing a Terraform null.
        /// </summary>
        /// <typeparam name="T">The generic type to create a Terraform null of.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TerraformValue<T> OfNull<T>(bool isUnknown) =>
            isUnknown ? TerraformValue<T>.Unknown : TerraformValue<T>.Null;

        /// <summary>
        /// Representing a Terraform null.
        /// </summary>
        /// <typeparam name="T">The generic type to create a Terraform null of.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TerraformValue<T> OfNull<T>() => TerraformValue<T>.Null;

        /// <summary>
        /// Representing a Terraform unknown.
        /// </summary>
        /// <typeparam name="T">The generic type to create a Terraform unknown of.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TerraformValue<T> OfUnknown<T>() => TerraformValue<T>.Unknown;
    }
}
