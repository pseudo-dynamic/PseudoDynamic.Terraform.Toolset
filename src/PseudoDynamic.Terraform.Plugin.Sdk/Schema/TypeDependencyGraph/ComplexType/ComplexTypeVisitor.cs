using System.Runtime.CompilerServices;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal class ComplexTypeVisitor
    {
        protected virtual VisitContext Rewrite(VisitContext context) =>
            context;

        private void VisitPropertySegment(IVisitPropertySegmentContext context)
        {
            var visitTypeGenericArguments = context.NullabilityInfo.NativeGenericTypeArguments;

            if (visitTypeGenericArguments is null || visitTypeGenericArguments.Length == 0) {
                return;
            }

            for (int i = 0; i < visitTypeGenericArguments.Length; i++) {
                var genericArgument = visitTypeGenericArguments[i];
                RewriteThenVisit(new VisitPropertyGenericSegmentContext(context, genericArgument, i));
            }
        }

        protected virtual void VisitPropertyGenericArgument(VisitPropertyGenericSegmentContext context) =>
            VisitPropertySegment(context);

        protected virtual void VisitProperty(VisitPropertyContext context) =>
            VisitPropertySegment(context);

        protected virtual void VisitComplex(VisitContext context)
        {
            context.RememberVisitTypeBeingVisited();
            var complexMetadata = context.ComplexTypeMetadata!;

            foreach (var property in complexMetadata.SupportedProperties) {
                RewriteThenVisit(new VisitPropertyContext(context, property));
            }
        }

        protected virtual void Visit(VisitContext context)
        {
            if (context.ContextType == VisitContextType.Complex) {
                VisitComplex(context);
            } else if (context.ContextType == VisitContextType.Property) {
                VisitProperty((VisitPropertyContext)context);
            } else if (context.ContextType == VisitContextType.PropertyGenericSegment) {
                VisitPropertyGenericArgument((VisitPropertyGenericSegmentContext)context);
            } else {
                throw new ArgumentException($"The context type is not supported: {context.ContextType.Id}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RewriteThenVisit(VisitContext context) =>
            Visit(Rewrite(context));

        public virtual void RewriteThenVisitDynamic(VisitContext context) =>
            RewriteThenVisit(context);

        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="complexType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void RewriteThenVisitComplex(Type complexType, Context? context = null)
        {
            if (complexType.IsGenericTypeDefinition) {
                throw new ArgumentException("The specified type must be a closed generic type");
            }

            if (!complexType.IsComplexType()) {
                throw new ArgumentException("The specified type must be a class type");
            }

            RewriteThenVisit(context.ToVisitingContext(complexType, VisitContextType.Complex));
        }

        public void RewriteThenVisitComplex<T>(Context? context = null) =>
            RewriteThenVisitComplex(typeof(T), context);
    }
}
