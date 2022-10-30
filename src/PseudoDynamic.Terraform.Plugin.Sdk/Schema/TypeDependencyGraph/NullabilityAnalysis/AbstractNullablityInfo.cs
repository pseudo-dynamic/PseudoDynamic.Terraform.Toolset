using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.NullabilityAnalysis
{
    internal abstract record AbstractNullablityInfo
    {
        public Type Type { get; internal init; }

        [AllowNull]
        public Type[] NativeGenericTypeArguments {
            get => _typeArguments ?? Type.GenericTypeArguments;
            init => _typeArguments = value;
        }

        public abstract NullabilityState ReadState { get; }

        public virtual AbstractNullablityInfo GetGenericTypeArgument(int index) =>
            new NullableInfo(NativeGenericTypeArguments[index]);

        private Type[]? _typeArguments;

        internal AbstractNullablityInfo(Type type) =>
            Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}
