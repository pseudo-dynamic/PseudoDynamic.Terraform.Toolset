using System.Diagnostics;
using static PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic.TypeVisitor;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    /// <summary>
    /// The current walking context.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    internal record VisitingContext : VisitorContext, IVisitingContext
    {
        /// <summary>
        /// The context type.
        /// </summary>
        public virtual VisitingContextType ContextType {
            get => _contextType;
            internal init => _contextType = value;
        }

        /// <summary>
        /// The type the walker is currently processing during walk.
        /// </summary>
        public virtual VisitingType VisitingType {
            get {
                return _visitingType ?? throw new InvalidOperationException("There is no type because you are not visiting");
            }
        }

        VisitingType IVisitingContext.VisitingType => VisitingType;

        private VisitingType? _visitingType;
        private VisitingContextType _contextType;

        internal VisitingContext(IVisitorContext context, Type walkingType)
            : base(context) =>
            SetWalkingType(walkingType);

        internal VisitingContext(Type walkingType) =>
            SetWalkingType(walkingType);

        public VisitingContext(VisitingContext context)
            : base(context) =>
            ApplyContext(context);

        internal VisitingContext(IVisitingContext context)
            : base(context) =>
            ApplyContext(context);

        private void SetWalkingType(Type walkingType)
        {
            if (walkingType is null) {
                throw new ArgumentNullException(nameof(walkingType));
            }

            _visitingType = new VisitingType(walkingType, this);
            CheckNonDependencyCycle(walkingType);
        }

        private void ApplyContext(IVisitingContext context)
        {
            _contextType = context.ContextType;
            _visitingType = context.VisitingType;
        }

        private void CheckNonDependencyCycle(Type type)
        {
            if (!type.IsComplexType()) {
                return;
            }

            if (VisitedComplexTypes.Contains(type)) {
                throw new TypeDependencyCycleException($"A type dependency cycle has been detected: {type.FullName}");
            }
        }

        private string GetDebuggerDisplay() =>
            $"[{ContextType}, {VisitingType.Type.Name}]";
    }
}
