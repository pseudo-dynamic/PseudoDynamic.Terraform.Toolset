﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether type is class or user-defined struct.
        /// </summary>
        /// <param name="type"></param>
        public static bool IsComplexType(this Type type) => type.IsClass;

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
