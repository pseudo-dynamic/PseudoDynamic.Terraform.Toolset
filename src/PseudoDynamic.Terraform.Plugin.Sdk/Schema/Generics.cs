using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal class Generics
    {
        public static bool IsTypeImplementingGenericTypeDefinition(
            Type type,
            Type genericTypeDefinition,
            [NotNullWhen(true)] out Type? implementerType,
            [NotNullWhen(true)] out Type[]? genericTypeArguments)
        {
            var interfaceTypes = type.GetInterfaces();

            foreach (var interfaceType in interfaceTypes) {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition) {
                    implementerType = type;
                    genericTypeArguments = interfaceType.GenericTypeArguments;
                    return true;
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition) {
                implementerType = type;
                genericTypeArguments = type.GenericTypeArguments;
                return true;
            }

            var baseType = type.BaseType;

            if (baseType == null) {
                implementerType = null;
                genericTypeArguments = null;
                return false;
            }

            return IsTypeImplementingGenericTypeDefinition(baseType, genericTypeDefinition, out implementerType, out genericTypeArguments);
        }
    }
}
