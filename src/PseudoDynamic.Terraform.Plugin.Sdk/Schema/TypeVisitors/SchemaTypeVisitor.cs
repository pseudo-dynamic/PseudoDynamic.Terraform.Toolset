using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal class SchemaTypeVisitor
    {
        private HierarchicalSchemaTypeVisitor _typeVisitor = new HierarchicalSchemaTypeVisitor();

        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="complexType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual SchemaTypeNode VisitSchema(Type complexType, VisitorContext? context = null)
        {
            if (!complexType.IsComplexType()) {
                throw new ArgumentException("The specified type must be either class or struct");
            }

            _typeVisitor.VisitComplex(complexType);
            return _typeVisitor.RootNode;
        }

        public SchemaTypeNode VisitSchema<T>(VisitorContext? context = null) =>
            VisitSchema(typeof(T), context);

        private class HierarchicalSchemaTypeVisitor : TypeVisitor
        {
            public SchemaTypeNode RootNode => _rootNode ?? throw new InvalidOperationException("You need to visit a type at least once to have access to root node");

            private SchemaTypeNode? _rootNode;
            private Queue<VisitingContext> _sameDepthCapturedContexts = new Queue<VisitingContext>();
            private bool _queueSameDepth;

            private bool IsSameDepthCapturingEnabled() => _queueSameDepth;

            private bool EnableSameDepthCapturing(bool enable = true) => _queueSameDepth = enable;

            private void CaptureSameDepthContext(VisitingContext context) => _sameDepthCapturedContexts.Enqueue(context);

            private List<VisitingContext> CaptureSameDepthContexts<T>(Action<T> visitChildren, T parentContext)
                where T : VisitingContext
            {
                EnableSameDepthCapturing();
                visitChildren(parentContext);
                EnableSameDepthCapturing(false);

                var capturedContexts = new List<VisitingContext>(_sameDepthCapturedContexts);
                _sameDepthCapturedContexts.Clear();
                return capturedContexts;
            }

            protected override void Visit(VisitingContext context)
            {
                if (IsSameDepthCapturingEnabled()) {
                    CaptureSameDepthContext(context);
                    return;
                }

                var capturedContexts = CaptureSameDepthContexts(base.Visit, context);
                var capturedContextNodes = capturedContexts.Select(context => new SchemaTypeNode(context)).ToArray();
                _rootNode = new SchemaTypeNode(context, capturedContextNodes);

                var postponedNodes = new Queue<SchemaTypeNode>(capturedContextNodes);
                while (postponedNodes.TryDequeue(out var node)) {
                    foreach (var capturedContext in CaptureSameDepthContexts(base.Visit, node.Context)) {
                        var capturedContextNode = new SchemaTypeNode(capturedContext);
                        postponedNodes.Enqueue(capturedContextNode);
                        node.Add(capturedContextNode);
                    }
                }
            }

            private bool TryVisitSchema(IVisitingPropertyArgumentContext context)
            {
                if (context.VisitingType.Type.IsSchemaAnnotated(out _)) {
                    Visit(new VisitingContext(context, context.VisitingType.Type) { ContextType = VisitingContextType.Complex });
                    return true;
                }

                return false;
            }

            protected override void VisitPropertyGenericArgument(VisitingPropertyGenericArgumentContext context)
            {
                if (TryVisitSchema(context)) {
                    return;
                }

                base.VisitPropertyGenericArgument(context);
            }

            protected override void VisitProperty(VisitingPropertyContext context)
            {
                if (TryVisitSchema(context)) {
                    return;
                }

                base.VisitProperty(context);
            }

            protected override void VisitComplex(VisitingContext context)
            {
                if (!context.VisitingType.Type.IsSchemaAnnotated(out _)) {
                    Visit(context.AsUnknown());
                    return;
                }

                base.VisitComplex(context);
            }

            public override void VisitComplex(Type complexType, VisitorContext? context = null)
            {
                if (!complexType.IsSchemaAnnotated(out _)) {
                    throw new ArgumentException($"Schema type must be annotated with [{typeof(SchemaAttribute).FullName}]");
                }

                base.VisitComplex(complexType, context);
            }
        }
    }
}
