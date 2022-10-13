using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Specialized;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Block
{
    internal class BlockNodeBuilder
    {
        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="blockType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual BlockNode BuildNode(Type blockType, Context? context = null)
        {
            var visitor = new SameDepthCapturingVisitor();
            visitor.VisitComplex(blockType);
            return visitor.RootNode;
        }

        public BlockNode BuildNode<T>(Context? context = null) =>
            BuildNode(typeof(T), context);

        private class SameDepthCapturingVisitor : ComplexTypeVisitor
        {
            public BlockNode RootNode => _rootNode ?? throw new InvalidOperationException("You need to visit a type at least once to have access to root node");

            private BlockNode? _rootNode;
            private SameDepthCapturing<VisitContext> _sameDepthCapturing = new SameDepthCapturing<VisitContext>();

            private bool TryRewritePropertySegment(IVisitPropertySegmentContext context, [NotNullWhen(true)] out VisitContext? rewrittenContext)
            {
                var visitedType = context.VisitedType;

                if (visitedType.IsBlockAnnotated(out _)) {
                    rewrittenContext = new VisitPropertyContext(context, context.VisitedType) { ContextType = VisitContextType.Complex.Inherits(context.ContextType) };
                    return true;
                }

                rewrittenContext = null;
                return false;
            }

            protected override VisitContext Rewrite(VisitContext context)
            {
                if (context is IVisitPropertySegmentContext propertySegmentContext && TryRewritePropertySegment(propertySegmentContext, out var rewrittenContext)) {
                    return rewrittenContext;
                }

                return context;
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

            private bool TryVisitPropertySegment(IVisitPropertySegmentContext context)
            {
                var visitedType = context.VisitedType;

                if (visitedType.IsImplementingGenericTypeDefinition(typeof(ITerraformValue<>), out _, out var genericTypeArguments)) {
                    var genericTypeArgument = genericTypeArguments.Single();

                    var annotatedTerraformValueGenericTypeParameters = visitedType.GetGenericTypeDefinition().GetGenericArguments()
                        .Select((GenericTypeParameter, Index) => (Index, GenericTypeParameter))
                        .Where(tuple => tuple.GenericTypeParameter.GetCustomAttribute<TerraformValueTypeAttribute>() is not null)
                        .ToArray();

                    if (annotatedTerraformValueGenericTypeParameters.Length == 0) {
                        throw new NotSupportedException($"Custom types implementing {typeof(ITerraformValue<>).FullName} without using {typeof(TerraformValueTypeAttribute).FullName} is not supported");
                    }

                    if (annotatedTerraformValueGenericTypeParameters.Length > 1) {
                        throw new TerraformValueException($"The generic type argument for {typeof(ITerraformValue<>).FullName} generic type definition can only originate from one generic type argument");
                    }

                    var terraformValueGenericTypeParameterTuple = annotatedTerraformValueGenericTypeParameters.Single();
                    var terraformValueGenericTypeArgumentIndex = terraformValueGenericTypeParameterTuple.Index;
                    var terraformValueGenericTypeArgument = visitedType.GetGenericArguments()[terraformValueGenericTypeArgumentIndex];

                    if (terraformValueGenericTypeArgument != genericTypeArgument) {
                        throw new TerraformValueException($@"The actual generic type argument of {typeof(ITerraformValue<>).FullName} generic type definition is incompatible with the indicated generic type argument of {visitedType.FullName}
actual generic type argument: {genericTypeArgument.FullName}
indicated generic type argument: {terraformValueGenericTypeArgument.FullName} (indicated by {typeof(TerraformValueTypeAttribute).FullName}");
                    }

                    Visit(new VisitPropertyGenericSegmentContext(context, terraformValueGenericTypeArgument, terraformValueGenericTypeArgumentIndex) { ContextType = TerraformVisitContextType.TerraformValue });
                    return true;
                }

                return false;
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
                if (!context.VisitedType.IsBlockAnnotated(out _)) {
                    return;
                }

                base.VisitComplex(context);
            }

            public override void VisitComplex(Type complexType, Context? context = null)
            {
                if (!complexType.IsBlockAnnotated(out _)) {
                    throw new ArgumentException($"Schema type must be annotated with [{typeof(BlockAttribute).FullName}]");
                }

                base.VisitComplex(complexType, context);
            }
        }
    }
}
