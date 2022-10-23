namespace PseudoDynamic.Terraform.Plugin
{
    internal class GenericTypeAccessor
    {
        private readonly Type _genericTypeDefinition;
        private Dictionary<Type, TypeAccessor> _typeAccessorByTypeArgument = new Dictionary<Type, TypeAccessor>();
        private Dictionary<Type[], TypeAccessor> _typeAccessorByTwoTypeArgument = new Dictionary<Type[], TypeAccessor>();

        public GenericTypeAccessor(Type genericTypeDefinition) =>
            _genericTypeDefinition = genericTypeDefinition ?? throw new ArgumentNullException(nameof(genericTypeDefinition));

        public TypeAccessor GetTypeAccessor(Type missingTypeArgument)
        {
            if (_typeAccessorByTypeArgument.TryGetValue(missingTypeArgument, out var typeAccessor)) {
                return typeAccessor;
            }

            typeAccessor = new TypeAccessor(_genericTypeDefinition.MakeGenericType(missingTypeArgument));
            _typeAccessorByTypeArgument[missingTypeArgument] = typeAccessor;
            return typeAccessor;
        }

        public TypeAccessor GetTypeAccessor(params Type[] missingTypeArguments)
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
