using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    internal class BlockNodeBuilder
    {
        internal static readonly BlockNodeBuilder Default = new BlockNodeBuilder();

        public virtual BlockNode BuildDynamic(VisitContext context)
        {
            var visitor = new SameDepthCapturingVisitor();
            visitor.RewriteThenVisitDynamic(context);
            return visitor.RootNode;
        }

        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="blockType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual BlockNode BuildNode(Type blockType, Context? context = null)
        {
            var visitor = new SameDepthCapturingVisitor();
            visitor.RewriteThenVisitComplex(blockType);
            return visitor.RootNode;
        }

        public BlockNode BuildNode<T>(Context? context = null) =>
            BuildNode(typeof(T), context);

        private class SameDepthCapturingVisitor : ComplexTypeVisitor
        {
            public BlockNode RootNode => _rootNode ?? throw new InvalidOperationException("You need to visit a type at least once to have access to root node");

            private BlockNode? _rootNode;
            private SameDepthCapturing<VisitContext> _sameDepthCapturing = new SameDepthCapturing<VisitContext>();

            private bool TryVisitPropertySegment(IVisitPropertySegmentContext context)
            {
                var visitType = context.VisitType;

                if (visitType == TerraformValue.BaseInterfaceType) {
                    RewriteThenVisitTerraformDynamic();
                } else if (visitType.IsImplementingGenericTypeDefinition(TerraformValue.InterfaceGenericTypeDefinition, out _, out var genericTypeArguments)) {
                    var genericTypeArgument = genericTypeArguments.Single();

                    var annotatedTerraformValueGenericTypeParameters = visitType.GetGenericTypeDefinition().GetGenericArguments()
                        .Select((GenericTypeParameter, Index) => (Index, GenericTypeParameter))
                        .Where(tuple => tuple.GenericTypeParameter.GetCustomAttribute<TerraformValueTypeAttribute>() is not null)
                        .ToArray();

                    if (annotatedTerraformValueGenericTypeParameters.Length == 0) {
                        throw new NotSupportedException($"Custom types implementing {TerraformValue.InterfaceGenericTypeDefinition.FullName} without using {typeof(TerraformValueTypeAttribute).FullName} is not supported");
                    }

                    if (annotatedTerraformValueGenericTypeParameters.Length > 1) {
                        throw new TerraformValueException($"The generic type argument for {TerraformValue.InterfaceGenericTypeDefinition.FullName} generic type definition can only originate from one generic type argument");
                    }

                    var terraformValueGenericTypeParameterTuple = annotatedTerraformValueGenericTypeParameters.Single();
                    var terraformValueGenericTypeArgumentIndex = terraformValueGenericTypeParameterTuple.Index;
                    var terraformValueGenericTypeArgument = visitType.GetGenericArguments()[terraformValueGenericTypeArgumentIndex];

                    if (terraformValueGenericTypeArgument != genericTypeArgument) {
                        throw new TerraformValueException($@"The actual generic type argument of {TerraformValue.InterfaceGenericTypeDefinition.FullName} generic type definition is incompatible with the indicated generic type argument of {visitType.FullName}
actual generic type argument: {genericTypeArgument.FullName}
indicated generic type argument: {terraformValueGenericTypeArgument.FullName} (indicated by {typeof(TerraformValueTypeAttribute).FullName}");
                    }

                    RewriteThenVisit(new VisitPropertyGenericSegmentContext(context, terraformValueGenericTypeArgument, terraformValueGenericTypeArgumentIndex) { ContextType = TerraformVisitContextType.TerraformValue });
                } else if (visitType.IsAssignableTo(TerraformValue.BaseInterfaceType)) {
                    RewriteThenVisitTerraformDynamic();
                } else {
                    return false;
                }

                return true;

                // Produces a Terraform dynamic
                void RewriteThenVisitTerraformDynamic() => RewriteThenVisit(VisitPropertyGenericSegmentContext.Custom(context, typeof(object)) with { ContextType = TerraformVisitContextType.TerraformValue });
            }

            protected override void VisitPropertyGenericArgument(VisitPropertyGenericSegmentContext context)
            {
                if (TryVisitPropertySegment(context)) {
                    return;
                }

                base.VisitPropertyGenericArgument(context);
            }

            protected override void VisitProperty(VisitPropertyContext context)
            {
                if (TryVisitPropertySegment(context)) {
                    return;
                }

                base.VisitProperty(context);
            }

            protected override void VisitComplex(VisitContext context)
            {
                if (!context.VisitType.IsComplexAnnotated(out _)) {
                    throw new MissingAttributeAnnotationException($"An attribute annotation was missing: {nameof(BlockAttribute)}, {nameof(ObjectAttribute)} or {nameof(TupleAttribute)}") {
                        ReceiverType = context.VisitType,
                        MissingAttributeType = typeof(ComplexAttribute)
                    };
                }

                base.VisitComplex(context);
            }

            protected override void Visit(VisitContext context)
            {
                if (_sameDepthCapturing.IsSameDepthCapturingEnabled) {
                    _sameDepthCapturing.CaptureSameDepth(context);
                    return;
                }

                var capturedContexts = _sameDepthCapturing.CaptureSameDepth(base.Visit, context);
                var capturedContextNodes = capturedContexts.Select(context => new BlockNode(context)).ToArray();
                _rootNode = new BlockNode(context, capturedContextNodes);

                var postponedNodes = new Queue<BlockNode>(capturedContextNodes);
                while (postponedNodes.TryDequeue(out var node)) {
                    foreach (var capturedContext in _sameDepthCapturing.CaptureSameDepth(base.Visit, node.Context)) {
                        var capturedContextNode = new BlockNode(capturedContext);
                        postponedNodes.Enqueue(capturedContextNode);
                        node.Add(capturedContextNode);
                    }
                }
            }

            private bool TryRewritePropertySegment<T>(T context, [NotNullWhen(true)] out T? rewrittenContext)
                where T : VisitContext, IVisitPropertySegmentContext
            {
                var visitedType = context.VisitType;

                if ((context.ImplicitTypeConstraints.Count == 1 && context.ImplicitTypeConstraints.Single().IsComplex())
                    || visitedType.IsComplexAnnotated(out _)) {
                    rewrittenContext = context with { ContextType = VisitContextType.Complex.Inherits(context.ContextType) };
                    return true;
                }

                rewrittenContext = null;
                return false;
            }

            protected override VisitPropertyContext RewriteProperty(VisitPropertyContext context)
            {
                if (TryRewritePropertySegment(context, out var rewrittenContext)) {
                    return rewrittenContext;
                }

                return context;
            }

            protected override VisitPropertyGenericSegmentContext RewritePropertyGenericArgument(VisitPropertyGenericSegmentContext context)
            {
                if (TryRewritePropertySegment(context, out var rewrittenContext)) {
                    return rewrittenContext;
                }

                return context;
            }

            public virtual void RewriteThenVisitDynamic(VisitContext context) =>
                RewriteThenVisit(context);

            public override void RewriteThenVisitComplex(Type complexType, Context? context = null)
            {
                if (!complexType.IsComplexAnnotated(out _)) {
                    throw new ArgumentException($"Schema type {complexType.FullName} must be annotated with [{typeof(BlockAttribute).FullName}]");
                }

                base.RewriteThenVisitComplex(complexType, context);
            }
        }
    }
}
