using System.Data;
using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal class ComplexTypeVisitor
    {
        protected virtual void Visit(VisitContext context)
        {
            if (context.ContextType == VisitContextType.Complex) {
                VisitComplex(context);
            } else if (context.ContextType == VisitContextType.Property) {
                VisitProperty((VisitPropertyContext)context);
            } else if (context.ContextType == VisitContextType.PropertyGenericArgument) {
                VisitPropertyGenericArgument((VisitPropertyGenericArgumentContext)context);
            } else {
                throw new ArgumentException($"The context type is not supported: {context.ContextType.Id}");
            }
        }

        private void VisitPropertySegment(IVisitPropertySegmentContext context)
        {
            if (context.VisitedType.TryGetGenericArguments(out var genericArguments)) {
                for (int i = 0; i < genericArguments.Length; i++) {
                    var genericArgument = genericArguments[i];
                    Visit(new VisitPropertyGenericArgumentContext(context, genericArgument, i));
                }
            }
        }

        protected virtual void VisitPropertyGenericArgument(VisitPropertyGenericArgumentContext context) =>
            VisitPropertySegment(context);

        protected virtual void VisitProperty(VisitPropertyContext context) =>
            VisitPropertySegment(context);

        protected virtual void VisitComplex(VisitContext context)
        {
            context.RememberTypeBeingVisited();

            // Get properties with public getters and setters
            var properties = context.VisitedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(x => x.GetGetMethod(nonPublic: false) != null && x.GetSetMethod(nonPublic: false) != null);

            foreach (var property in properties) {
                Visit(new VisitPropertyContext(context, property));
            }
        }

        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="complexType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void VisitComplex(Type complexType, RootVisitContext? context = null)
        {
            if (complexType.IsGenericTypeDefinition) {
                throw new ArgumentException("The specified type must be a closed generic type");
            }

            if (!complexType.IsComplexType()) {
                throw new ArgumentException("The specified type must be either class or struct");
            }

            Visit(context.ToVisitingContext(complexType, VisitContextType.Complex));
        }

        public void VisitComplexType<T>(RootVisitContext? context = null) =>
            VisitComplex(typeof(T), context);
    }
}
