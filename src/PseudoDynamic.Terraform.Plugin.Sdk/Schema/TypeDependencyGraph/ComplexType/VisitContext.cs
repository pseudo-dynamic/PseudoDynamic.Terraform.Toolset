using System.Collections.Immutable;
using System.Diagnostics;
using Namotion.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{

    /// <summary>
    /// The context originated from a visitation.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    internal record VisitContext : Context, IVisitContext, IMaybeComplexType
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

            internal init {
                var visitType = _visitType;
                _visitType = value;

                if (visitType != value) {
                    _implicitTypeConstraints = null;
                    _complexMetadata = null;
                    _visitedContextualType = null;
                }
            }
        }

        Type IVisitContext.VisitType => VisitType;
        Type IMaybeComplexType.Type => VisitType;

        public ComplexTypeMetadata? ComplexTypeMetadata {
            get {
                if (ContextType != VisitContextType.Complex) {
                    throw new NotSupportedException($@"Only contexts of type {VisitContextType.Complex.Id} can have complex metadata
Context type = {ContextType}
Visit type = {VisitType.FullName}");
                }

                return _complexMetadata ??= ComplexTypeMetadata.FromVisitContext(this);
            }
        }

        [MemberNotNullWhen(true, nameof(ComplexTypeMetadata))]
        public bool HasComplexTypeMetadata => ContextType == VisitContextType.Complex;

        public IReadOnlySet<TerraformTypeConstraint> ImplicitTypeConstraints {
            get {
                var implicitTypeConstraints = _implicitTypeConstraints;

                if (implicitTypeConstraints is not null) {
                    return implicitTypeConstraints;
                }

                if (GetVisitTypeAttribute<SkipImplicitTypeConstraintEvaluationAttribute>() is not null) {
                    implicitTypeConstraints = ImmutableHashSet<TerraformTypeConstraint>.Empty;
                } else {
                    implicitTypeConstraints = TerraformTypeConstraintEvaluator.Default.Evaluate(VisitType);
                }

                _implicitTypeConstraints = implicitTypeConstraints;
                return implicitTypeConstraints;
            }
        }

        bool IMaybeComplexType.IsComplexType => _isComplexType;

        private VisitContextType? _contextType;
        private Type? _visitType;
        private ContextualType? _visitedContextualType;
        private ComplexTypeMetadata? _complexMetadata;
        private IReadOnlySet<TerraformTypeConstraint>? _implicitTypeConstraints;
        private bool _isComplexType;

        internal VisitContext(IContext context, Type visitType)
            : base(context) =>
            SetVisitType(visitType);

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
            _isComplexType = visitType.IsComplexType();
            CheckNonDependencyCycle();

            void CheckNonDependencyCycle()
            {
                if (!_isComplexType) {
                    return;
                }

                if (RememberedComplexVisitTypes.Contains(_visitType)) {
                    throw new TypeDependencyCycleException() { Type = _visitType };
                }
            }
        }

        private void ApplyContext(IVisitContext context)
        {
            _contextType = context.ContextType;
            _visitType = context.VisitType;
            _implicitTypeConstraints = context.ImplicitTypeConstraints;
            _isComplexType = context.IsComplexType;
        }

        internal void RememberVisitTypeBeingVisited() =>
            RememberComplexTypeBeingVisited(this);

        private ContextualType GetVisitedContextualType() =>
            _visitedContextualType ??= VisitType.ToContextualType();

        /// <summary>
        /// Gets the contextual attribute from maybe the visited type or property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T? GetVisitTypeAttribute<T>()
            where T : Attribute =>
            GetVisitedContextualType().GetInheritedAttribute<T>();

        /// <summary>
        /// Gets the contextual attribute from maybe the visited type or property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual T? GetContextualAttribute<T>()
            where T : Attribute =>
            GetVisitTypeAttribute<T>();

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
