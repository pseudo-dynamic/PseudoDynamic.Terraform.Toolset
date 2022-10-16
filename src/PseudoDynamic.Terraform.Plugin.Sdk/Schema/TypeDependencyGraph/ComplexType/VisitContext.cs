using System.Diagnostics;
using Namotion.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    /// <summary>
    /// The current walking context.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    internal record VisitContext : Context, IVisitContext
    {
        /// <summary>
        /// The context type.
        /// </summary>
        public virtual VisitContextType ContextType {
            get => _contextType ?? throw new InvalidOperationException("Context type was not set");
            internal init => _contextType = value;
        }

        /// <summary>
        /// The type the walker is currently processing during walk.
        /// </summary>
        public virtual Type VisitedType {
            get {
                return _visitType ?? throw new InvalidOperationException("There is no type because you are not visiting");
            }
        }

        Type IVisitContext.VisitedType => VisitedType;

        private VisitContextType? _contextType;
        private Type? _visitType;
        private ContextualType? _visitedContextualType;

        internal VisitContext(IContext context, Type visitedType)
            : base(context) =>
            SetVisitedType(visitedType);

        internal VisitContext(Type walkingType) =>
            SetVisitedType(walkingType);

        public VisitContext(VisitContext context)
            : base(context) =>
            ApplyContext(context);

        internal VisitContext(IVisitContext context)
            : base(context) =>
            ApplyContext(context);

        private void SetVisitedType(Type visitType)
        {
            if (visitType is null) {
                throw new ArgumentNullException(nameof(visitType));
            }

            _visitType = visitType;
            CheckNonDependencyCycle(visitType);
        }

        private void ApplyContext(IVisitContext context)
        {
            _contextType = context.ContextType;
            _visitType = context.VisitedType;
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

        internal void RememberTypeBeingVisited() =>
            AddVisitedComplexType(VisitedType);

        private ContextualType GetVisitedContextualType() =>
            _visitedContextualType ??= VisitedType.ToContextualType();

        /// <summary>
        /// Gets the contextual attribute from maybe the visited type or property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T? GetVisitedTypeAttribute<T>()
            where T : Attribute =>
            GetVisitedContextualType().GetInheritedAttribute<T>();

        /// <summary>
        /// Gets the contextual attribute from maybe the visited type or property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual T? GetContextualAttribute<T>()
            where T : Attribute =>
            GetVisitedTypeAttribute<T>();

        private string GetDebuggerDisplay() =>
            $"[{ContextType}, {VisitedType.Name}]";
    }
}
