using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal class TypeVisitor
    {
        protected virtual void Visit(VisitingContext context)
        {
            if (context.ContextType == VisitingContextType.Complex) {
                VisitComplex(context);
            } else if (context.ContextType == VisitingContextType.Property) {
                VisitProperty((VisitingPropertyContext)context);
            } else if (context.ContextType == VisitingContextType.PropertyGenericArgument) {
                VisitPropertyGenericArgument((VisitingPropertyGenericArgumentContext)context);
            } else if (context.ContextType == VisitingContextType.Unknown) {
                VisitUnknown(context);
            } else {
                throw new ArgumentException("The context type is not supported");
            }
        }

        protected virtual void VisitUnknown(VisitingContext context)
        {
        }

        private void VisitPropertyArgument(IVisitingPropertyArgumentContext context)
        {
            if (context.VisitingType.TryGetGenericArguments(out var genericArguments)) {
                for (int i = 0; i < genericArguments.Length; i++) {
                    var genericArgument = genericArguments[i];
                    Visit(new VisitingPropertyGenericArgumentContext(context, genericArgument, i));
                }
            }
            //else {
            //    Visit(context.AsUnknown());
            //}
        }

        protected virtual void VisitPropertyGenericArgument(VisitingPropertyGenericArgumentContext context) =>
            VisitPropertyArgument(context);

        protected virtual void VisitProperty(VisitingPropertyContext context) =>
            VisitPropertyArgument(context);

        protected virtual void VisitComplex(VisitingContext context)
        {
            context.VisitingType.RememberBeingVisited();

            // Get properties with public getters and setters
            var properties = context.VisitingType.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(x => x.GetGetMethod(nonPublic: false) != null && x.GetSetMethod(nonPublic: false) != null);

            foreach (var property in properties) {
                Visit(new VisitingPropertyContext(context, property));
            }
        }

        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="complexType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void VisitComplex(Type complexType, VisitorContext? context = null)
        {
            if (complexType.IsGenericTypeDefinition) {
                throw new ArgumentException("The specified type must be a closed generic type");
            }

            if (!complexType.IsComplexType()) {
                throw new ArgumentException("The specified type must be either class or struct");
            }

            Visit(context.ToVisitingContext(complexType, VisitingContextType.Complex));
        }

        public void VisitComplexType<T>(VisitorContext? context = null) =>
            VisitComplex(typeof(T), context);

        public readonly struct VisitingType
        {
            public Type Type { get; }

            private readonly IVisitedComplexTypes _visitedComplexTypes;

            internal VisitingType(Type type, IVisitedComplexTypes visitedComplexTypes)
            {
                Type = type;
                _visitedComplexTypes = visitedComplexTypes;
            }

            internal void RememberBeingVisited() => _visitedComplexTypes.AddVisitedComplexType(Type);

            public bool TryGetGenericArguments([NotNullWhen(true)] out Type[]? genericArguments)
            {
                if (!Type.IsConstructedGenericType) {
                    genericArguments = null;
                    return false;
                }

                genericArguments = Type.GetGenericArguments();
                return true;
            }
        }
    }
}
