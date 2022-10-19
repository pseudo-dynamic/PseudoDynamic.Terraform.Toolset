using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin
{
    internal class GenericTypeAccessor
    {
        private readonly Type _genericTypeDefinition;
        private ConstructorAccessor? _constructorAccessor;

        public GenericTypeAccessor(Type genericTypeDefinition) =>
            _genericTypeDefinition = genericTypeDefinition ?? throw new ArgumentNullException(nameof(genericTypeDefinition));

        public object CreateInstance(Type typeArgument, params object?[] args) =>
            (_constructorAccessor ??= new(_genericTypeDefinition)).GetConstructor(typeArgument, args.Length).Invoke(args);

        private class ConstructorAccessor
        {
            private readonly Type _genericTypeDefinition;
            private readonly Dictionary<Type, ConstructorInfo> _cachedConstructors = new Dictionary<Type, ConstructorInfo>();

            public ConstructorInfo GetConstructor(Type valueType, int argsLength)
            {
                if (_cachedConstructors.TryGetValue(valueType, out var cachedConstructor)) {
                    return cachedConstructor;
                }

                var genericType = _genericTypeDefinition.MakeGenericType(valueType);
                var constructor = genericType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.GetParameters().Length == argsLength);
                _cachedConstructors[valueType] = constructor;
                return constructor;
            }

            public ConstructorAccessor(Type genericTypeDefinition) =>
                _genericTypeDefinition = genericTypeDefinition;
        }
    }
}
