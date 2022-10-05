using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    /// <summary>
    /// Represents the initial context of a visitation.
    /// </summary>
    public record VisitorContext : IVisitorContext, IVisitedComplexTypes
    {
        public IReadOnlySet<Type> VisitedComplexTypes => _visitedComplexTypes;
        public NullabilityInfoContext NullabilityInfoContext => _nullabilityInfoContext;

        private HashSet<Type> _visitedComplexTypes;
        private NullabilityInfoContext _nullabilityInfoContext;

        internal VisitorContext(IVisitorContext context) =>
            ApplyContext(context);

        public VisitorContext(VisitorContext context) =>
            ApplyContext(context);

        public VisitorContext()
        {
            _visitedComplexTypes = new HashSet<Type>();
            _nullabilityInfoContext = new NullabilityInfoContext();
        }

        [MemberNotNull(nameof(_visitedComplexTypes), nameof(_nullabilityInfoContext))]
        private void ApplyContext(IVisitorContext context)
        {
            _visitedComplexTypes = new HashSet<Type>(context.VisitedComplexTypes);
            _nullabilityInfoContext = context.NullabilityInfoContext;
        }

        public void AddVisitedComplexType(Type type)
        {
            if (!type.IsComplexType()) {
                return;
            }

            _visitedComplexTypes.Add(type);
        }
    }
}
