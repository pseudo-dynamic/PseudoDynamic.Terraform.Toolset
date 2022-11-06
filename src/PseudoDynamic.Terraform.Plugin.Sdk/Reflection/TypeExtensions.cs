namespace PseudoDynamic.Terraform.Plugin.Reflection
{
    internal static class TypeExtensions
    {
        public static bool IsImplementingGenericTypeDefinition(
            this Type type,
            Type genericTypeDefinition,
            [NotNullWhen(true)] out Type? genericType,
            [NotNullWhen(true)] out Type[]? genericTypeArguments)
        {
            foreach (Type interfaceType in type.GetInterfaces()) {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition) {
                    genericType = interfaceType;
                    genericTypeArguments = interfaceType.GenericTypeArguments;
                    return true;
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition) {
                genericType = type;
                genericTypeArguments = type.GenericTypeArguments;
                return true;
            }

            Type? baseType = type.BaseType;

            if (baseType == null) {
                genericType = null;
                genericTypeArguments = null;
                return false;
            }

            return baseType.IsImplementingGenericTypeDefinition(genericTypeDefinition, out genericType, out genericTypeArguments);
        }
    }
}
