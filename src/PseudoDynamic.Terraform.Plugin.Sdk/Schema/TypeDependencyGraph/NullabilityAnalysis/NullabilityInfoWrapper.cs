using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.NullabilityAnalysis
{
    internal record NullabilityInfoWrapper : AbstractNullablityInfo
    {
        public override NullabilityState ReadState => _nullabilityInfo.ReadState;

        private readonly NullabilityInfo _nullabilityInfo;
        private Type[]? genericTypeArgumentsFallback;

        internal NullabilityInfoWrapper(NullabilityInfo nullabilityInfo) : base(nullabilityInfo.Type) =>
            _nullabilityInfo = nullabilityInfo ?? throw new ArgumentNullException(nameof(nullabilityInfo));

        public override AbstractNullablityInfo GetGenericTypeArgument(int index)
        {
            // If you fetch for an index and no generic type arguments are present inside
            // the native nullability info, then we fetch generic type arguments manually
            if (_nullabilityInfo.GenericTypeArguments.Length == 0) {
                genericTypeArgumentsFallback ??= _nullabilityInfo.Type.GenericTypeArguments;
                return new NullableInfo(genericTypeArgumentsFallback[index]);
            }

            return new NullabilityInfoWrapper(_nullabilityInfo.GenericTypeArguments[index]);
        }
    }
}
