using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class TerraformEqualityEqualityComparer<T> : EqualityComparer<ITerraformValue<T>>
    {
        private static readonly TerraformEqualityEqualityComparer<T>? _default;

        public static new TerraformEqualityEqualityComparer<T> Default = _default ??= new TerraformEqualityEqualityComparer<T>(EqualityComparer<T>.Default);
        private readonly IEqualityComparer<T> _equalityComparer;

        public TerraformEqualityEqualityComparer(IEqualityComparer<T>? equalityComparer) =>
            _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

        public override bool Equals(ITerraformValue<T>? x, ITerraformValue<T>? y) =>
            object.Equals(x, y)
            || (x is not null && y is not null
                && _equalityComparer.Equals(x.Value, y.Value)
                && x.IsNull == y.IsNull
                && y.IsUnknown == y.IsUnknown);

        public override int GetHashCode([DisallowNull] ITerraformValue<T> obj)
        {
            throw new NotImplementedException();
        }
    }
}
