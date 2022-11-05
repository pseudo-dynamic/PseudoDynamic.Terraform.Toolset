using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    /// <summary>
    /// Represents the initial context of a visitation.
    /// </summary>
    internal record Context : IContext
    {
        public IReadOnlySet<Type> RememberedComplexVisitTypes => _rememberedComplexVisitTypes;
        public NullabilityInfoContext NullabilityInfoContext => _nullabilityInfoContext;

        private HashSet<Type> _rememberedComplexVisitTypes;
        private NullabilityInfoContext _nullabilityInfoContext;

        internal Context(IContext context) =>
            ApplyContext(context);

        public Context(Context context) =>
            ApplyContext(context);

        public Context()
        {
            _rememberedComplexVisitTypes = new HashSet<Type>();
            _nullabilityInfoContext = new NullabilityInfoContext();
        }

        [MemberNotNull(nameof(_rememberedComplexVisitTypes), nameof(_nullabilityInfoContext))]
        private void ApplyContext(IContext context)
        {
            _rememberedComplexVisitTypes = new HashSet<Type>(context.RememberedComplexVisitTypes);
            _nullabilityInfoContext = context.NullabilityInfoContext;
        }

        protected internal void RememberComplexTypeBeingVisited(IMaybeComplexType complexType)
        {
            if (!complexType.IsComplexType) {
                return;
            }

            _rememberedComplexVisitTypes.Add(complexType.Type);
        }
    }
}
