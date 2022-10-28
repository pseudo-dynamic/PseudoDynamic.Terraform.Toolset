using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal record InheritableNullabilityInfo
    {
        public Type Type { get; internal init; }

        public virtual NullabilityState ReadState => NullabilityState.Nullable;

        public virtual InheritableNullabilityInfo GetGenericTypeArgument(int index)
        {
            var typeArguments = _typeArguments ??= Type.GenericTypeArguments;
            return new InheritableNullabilityInfo(typeArguments[index]);
        }

        private Type[]? _typeArguments;

        internal InheritableNullabilityInfo(Type type) =>
            Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}
