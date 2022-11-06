using System.Runtime.CompilerServices;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal class ComplexTypeVisitor
    {
        internal static readonly ComplexTypeVisitor Default = new();

        private void VisitPropertySegment(IVisitPropertySegmentContext context)
        {
            Type[]? visitTypeGenericArguments = context.NullabilityInfo.GenericTypeArguments;

            if (visitTypeGenericArguments is null || visitTypeGenericArguments.Length == 0) {
                return;
            }

            for (int i = 0; i < visitTypeGenericArguments.Length; i++) {
                Type genericArgument = visitTypeGenericArguments[i];
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
            ComplexTypeMetadata complexMetadata = context.ComplexTypeMetadata!;

            foreach (System.Reflection.PropertyInfo property in complexMetadata.SupportedProperties) {
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

        protected virtual VisitContext RewriteComplex(VisitContext context) =>
            context;

        private T RewritePropertySegment<T>(T context)
            where T : VisitContext, IVisitPropertySegmentContext
        {
            if (context.VisitType.IsComplexType()) {
                return context with { ContextType = VisitContextType.Complex.Inherits(context.ContextType) };
            }

            return context;
        }

        protected virtual VisitPropertyContext RewriteProperty(VisitPropertyContext context) =>
            RewritePropertySegment(context);

        protected virtual VisitPropertyGenericSegmentContext RewritePropertyGenericArgument(VisitPropertyGenericSegmentContext context) =>
            RewritePropertySegment(context);

        protected virtual VisitContext Rewrite(VisitContext context)
        {
            if (context.ContextType == VisitContextType.Complex) {
                return RewriteComplex(context);
            } else if (context.ContextType == VisitContextType.Property) {
                return RewriteProperty((VisitPropertyContext)context);
            } else if (context.ContextType == VisitContextType.PropertyGenericSegment) {
                return RewritePropertyGenericArgument((VisitPropertyGenericSegmentContext)context);
            } else {
                throw new ArgumentException($"The context type is not supported: {context.ContextType.Id}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RewriteThenVisit(VisitContext context) =>
            Visit(Rewrite(context));

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

            if (!complexType.IsClassType()) {
                throw new ArgumentException("The specified type must be a class type");
            }

            RewriteThenVisit(context.ToVisitingContext(complexType, VisitContextType.Complex));
        }

        public void RewriteThenVisitComplex<T>(Context? context = null) =>
            RewriteThenVisitComplex(typeof(T), context);
    }
}
