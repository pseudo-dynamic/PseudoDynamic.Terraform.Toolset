using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal record CustomNullabilityInfo
    {
        public Type Type { get; internal init; }

        [AllowNull]
        public Type[] NativeGenericTypeArguments {
            get => _typeArguments ?? Type.GenericTypeArguments;
            init => _typeArguments = value;
        }

        public virtual NullabilityState ReadState => NullabilityState.Nullable;

        public virtual CustomNullabilityInfo GetGenericTypeArgument(int index) =>
            new CustomNullabilityInfo(NativeGenericTypeArguments[index]);

        private Type[]? _typeArguments;

        internal CustomNullabilityInfo(Type type) =>
            Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}
