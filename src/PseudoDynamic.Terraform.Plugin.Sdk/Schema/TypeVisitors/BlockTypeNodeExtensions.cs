using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal static class BlockTypeNodeExtensions
    {
        //public static bool IsImplementingGenericTypeDefinition<TBlockTypeNode>(
        //    this TBlockTypeNode type,
        //    Type genericTypeDefinition,
        //    [NotNullWhen(true)] out Type? genericType,
        //    [NotNullWhen(true)] out Type[]? genericTypeArguments)
        //    where TBlockTypeNode : BlockTypeNodeBase<TBlockTypeNode>
        //{
        //    foreach (var interfaceType in type.Context.VisitedType.GetInterfaces()) {
        //        if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition) {
        //            genericType = interfaceType;
        //            genericTypeArguments = interfaceType.GenericTypeArguments;
        //            return true;
        //        }
        //    }

        //    if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition) {
        //        genericType = type;
        //        genericTypeArguments = type.GenericTypeArguments;
        //        return true;
        //    }

        //    var baseType = type.BaseType;

        //    if (baseType == null) {
        //        genericType = null;
        //        genericTypeArguments = null;
        //        return false;
        //    }

        //    return IsImplementingGenericTypeDefinition(baseType, genericTypeDefinition, out genericType, out genericTypeArguments);
        //}
    }
}
