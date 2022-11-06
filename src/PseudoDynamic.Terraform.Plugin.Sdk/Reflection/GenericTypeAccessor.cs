namespace PseudoDynamic.Terraform.Plugin.Reflection
{
    internal class GenericTypeAccessor
    {
        private readonly Type _genericTypeDefinition;
        private readonly Dictionary<Type, TypeAccessor> _typeAccessorByTypeArgument = new();
        private readonly Dictionary<Type[], TypeAccessor> _typeAccessorByTwoTypeArgument = new();

        public GenericTypeAccessor(Type genericTypeDefinition) =>
            _genericTypeDefinition = genericTypeDefinition ?? throw new ArgumentNullException(nameof(genericTypeDefinition));

        public TypeAccessor MakeGenericTypeAccessor(Type missingTypeArgument)
        {
            if (_typeAccessorByTypeArgument.TryGetValue(missingTypeArgument, out var typeAccessor)) {
                return typeAccessor;
            }

            typeAccessor = new TypeAccessor(_genericTypeDefinition.MakeGenericType(missingTypeArgument));
            _typeAccessorByTypeArgument[missingTypeArgument] = typeAccessor;
            return typeAccessor;
        }

        public TypeAccessor MakeGenericTypeAccessor(params Type[] missingTypeArguments)
        {
            if (_typeAccessorByTwoTypeArgument.TryGetValue(missingTypeArguments, out var typeAccessor)) {
                return typeAccessor;
            }

            typeAccessor = new TypeAccessor(_genericTypeDefinition.MakeGenericType(missingTypeArguments));
            _typeAccessorByTwoTypeArgument[missingTypeArguments] = typeAccessor;
            return typeAccessor;
        }
    }
}
