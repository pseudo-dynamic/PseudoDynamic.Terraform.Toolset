using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors;
using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class SchemaTypeNodeEqualityComparer : EqualityComparer<SchemaTypeNode>
    {
        public new static readonly SchemaTypeNodeEqualityComparer Default = new SchemaTypeNodeEqualityComparer();

        public override bool Equals(SchemaTypeNode? x, SchemaTypeNode? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Context.ContextType == y.Context.ContextType
                && x.Context.VisitingType.Type == y.Context.VisitingType.Type
                && x.Nodes.SequenceEqual(y.Nodes, Default);
        }

        public override int GetHashCode([DisallowNull] SchemaTypeNode obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.Context.ContextType);
            hashCode.Add(obj.Context.VisitingType.Type);
            return hashCode.ToHashCode();
        }
    }
}
