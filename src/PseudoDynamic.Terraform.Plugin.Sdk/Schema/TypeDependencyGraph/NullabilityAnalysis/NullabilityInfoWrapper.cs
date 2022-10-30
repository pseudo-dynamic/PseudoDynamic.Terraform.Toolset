using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.NullabilityAnalysis
{
    internal record NullabilityInfoWrapper : AbstractNullablityInfo
    {
        public override NullabilityState ReadState => _nullabilityInfo.ReadState;

        private readonly NullabilityInfo _nullabilityInfo;

        internal NullabilityInfoWrapper(NullabilityInfo nullabilityInfo) : base(nullabilityInfo.Type) =>
            _nullabilityInfo = nullabilityInfo ?? throw new ArgumentNullException(nameof(nullabilityInfo));

        public override AbstractNullablityInfo GetGenericTypeArgument(int index) =>
            new NullabilityInfoWrapper(_nullabilityInfo.GenericTypeArguments[index]);
    }
}
