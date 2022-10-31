using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether type is a class type.
        /// </summary>
        /// <param name="type"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsClassType(this Type type) => type.IsClass;

        /// <summary>
        /// Determines whether type is a class type.
        /// </summary>
        /// <param name="type"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUserDefinedStruct(this Type type) =>
            type.IsValueType && !type.IsEnum
            && !type.IsEquivalentTo(typeof(decimal))
            && !type.IsPrimitive;

        /// <summary>
        /// Determines whether type is a class type or a user-defined struct.
        /// </summary>
        /// <param name="type"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsComplexType(this Type type) =>
            type.IsClass || type.IsUserDefinedStruct();

        /// <summary>
        /// Determines if type is a annotated by <see cref="BlockAttribute"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        public static bool IsComplexAnnotated(this Type type, [NotNullWhen(true)] out ComplexAttribute? attribute)
        {
            attribute = type.GetCustomAttribute<ComplexAttribute>();
            return attribute is not null;
        }

        public static bool TryGetGenericArguments(this Type type, [NotNullWhen(true)] out Type[]? genericArguments)
        {
            if (!type.IsConstructedGenericType) {
                genericArguments = null;
                return false;
            }

            genericArguments = type.GetGenericArguments();
            return true;
        }
    }
}
