using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.NullabilityAnalysis
{
    internal record NullableInfo : AbstractNullablityInfo
    {
        public NullableInfo(Type type) : base(type)
        {
        }

        public override NullabilityState ReadState => NullabilityState.Nullable;
    }
}
