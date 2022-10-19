using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Namotion.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    /// <summary>
    /// The context originated from a visitation.
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
        /// The type the visitor has been processed.
        /// </summary>
        public virtual Type VisitType {
            get {
                return _visitType ?? throw new InvalidOperationException("There is no type because you are not visiting");
            }
        }

        public ComplexTypeMetadata? ComplexTypeMetadata {
            get {
                if (ContextType != VisitContextType.Complex) {
                    throw new NotSupportedException($"Other contexts of type {VisitContextType.Complex.Id} cannot possess complex metadata");
                }

                return _complexMetadata ??= ComplexTypeMetadata.FromVisitContext(this);
            }
        }

        [MemberNotNullWhen(true, nameof(ComplexTypeMetadata))]
        public bool HasComplexTypeMetadata => ContextType == VisitContextType.Complex;

        Type IVisitContext.VisitType => VisitType;

        private VisitContextType? _contextType;
        private Type? _visitType;
        private ContextualType? _visitedContextualType;
        private ComplexTypeMetadata? _complexMetadata;
        private bool _computedComplexMetadata;

        internal VisitContext(IContext context, Type visitedType)
            : base(context) =>
            SetVisitType(visitedType);

        internal VisitContext(Type walkingType) =>
            SetVisitType(walkingType);

        public VisitContext(VisitContext context)
            : base(context) =>
            ApplyContext(context);

        internal VisitContext(IVisitContext context)
            : base(context) =>
            ApplyContext(context);

        private void SetVisitType(Type visitType)
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
            _visitType = context.VisitType;
        }

        private void CheckNonDependencyCycle(Type type)
        {
            if (!type.IsComplexType()) {
                return;
            }

            if (RememberedComplexVisitTypes.Contains(type)) {
                throw new TypeDependencyCycleException($"A type dependency cycle has been detected: {type.FullName}");
            }
        }

        internal void RememberVisitTypeBeingVisited() =>
            RememberComplexTypeBeingVisited(VisitType);

        private ContextualType GetVisitedContextualType() =>
            _visitedContextualType ??= VisitType.ToContextualType();

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

        public virtual bool Equals(VisitContext? context) =>
            context is not null
            && ContextType == context.ContextType
            && VisitType == context.VisitType;

        public override int GetHashCode() => HashCode.Combine(
            ContextType,
            VisitType);

        private string GetDebuggerDisplay() =>
            $"[{ContextType}, {VisitType.Name}]";
    }
}
