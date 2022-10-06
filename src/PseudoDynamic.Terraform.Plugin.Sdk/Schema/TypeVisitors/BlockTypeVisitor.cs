using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;
using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal class BlockTypeVisitor : ComplexTypeVisitor
    {
        public BlockTypeNode RootNode => _rootNode ?? throw new InvalidOperationException("You need to visit a type at least once to have access to root node");

        private BlockTypeNode? _rootNode;
        private Queue<VisitContext> _sameDepthCapturedContexts = new Queue<VisitContext>();
        private bool _queueSameDepth;

        private bool IsSameDepthCapturingEnabled() => _queueSameDepth;

        private bool EnableSameDepthCapturing(bool enable = true) => _queueSameDepth = enable;

        private void CaptureSameDepthContext(VisitContext context) => _sameDepthCapturedContexts.Enqueue(context);

        private List<VisitContext> CaptureSameDepthContexts<T>(Action<T> visitChildren, T parentContext)
            where T : VisitContext
        {
            EnableSameDepthCapturing();
            visitChildren(parentContext);
            EnableSameDepthCapturing(false);

            var capturedContexts = new List<VisitContext>(_sameDepthCapturedContexts);
            _sameDepthCapturedContexts.Clear();
            return capturedContexts;
        }

        protected virtual void TerraformVisit(VisitContext context) =>
            base.Visit(context);

        protected override void Visit(VisitContext context)
        {
            if (IsSameDepthCapturingEnabled()) {
                CaptureSameDepthContext(context);
                return;
            }

            var capturedContexts = CaptureSameDepthContexts(TerraformVisit, context);
            var capturedContextNodes = capturedContexts.Select(context => new BlockTypeNode(context)).ToArray();
            _rootNode = new BlockTypeNode(context, capturedContextNodes);

            var postponedNodes = new Queue<BlockTypeNode>(capturedContextNodes);
            while (postponedNodes.TryDequeue(out var node)) {
                foreach (var capturedContext in CaptureSameDepthContexts(TerraformVisit, node.Context)) {
                    var capturedContextNode = new BlockTypeNode(capturedContext);
                    postponedNodes.Enqueue(capturedContextNode);
                    node.Add(capturedContextNode);
                }
            }
        }

        private bool TryVisitPropertySegment(IVisitPropertySegmentContext context)
        {
            var visitedType = context.VisitedType;

            if (visitedType.IsBlockAnnotated(out _)) {
                Visit(new VisitContext(context, context.VisitedType) { ContextType = VisitContextType.Complex });
                return true;
            }

            if (visitedType.IsImplementingGenericTypeDefinition(typeof(ITerraformValue<>), out _, out var genericTypeArguments)) {
                var genericTypeArgument = genericTypeArguments.Single();

                var annotatedTerraformValueGenericTypeParameters = visitedType.GetGenericTypeDefinition().GetGenericArguments()
                    .Select((GenericTypeParameter, Index) => (Index, GenericTypeParameter))
                    .Where(tuple => tuple.GenericTypeParameter.GetCustomAttribute<TerraformValueAttribute>() is not null)
                    .ToArray();

                if (annotatedTerraformValueGenericTypeParameters.Length != 0) {
                    if (annotatedTerraformValueGenericTypeParameters.Length != 1) {
                        throw new TerraformValueException($"The generic type argument for {typeof(ITerraformValue<>).FullName} generic type definition can only originate from one generic type argument");
                    }

                    var terraformValueGenericTypeParameterTuple = annotatedTerraformValueGenericTypeParameters.Single();
                    var terraformValueGenericTypeArgumentIndex = terraformValueGenericTypeParameterTuple.Index;
                    var terraformValueGenericTypeArgument = visitedType.GetGenericArguments()[terraformValueGenericTypeArgumentIndex];

                    if (terraformValueGenericTypeArgument != genericTypeArgument) {
                        throw new TerraformValueException($@"The actual generic type argument of {typeof(ITerraformValue<>).FullName} generic type definition is incompatible with the indicated generic type argument of {visitedType.FullName}
actual generic type argument: {genericTypeArgument.FullName}
indicated generic type argument: {terraformValueGenericTypeArgument.FullName} (indicated by {typeof(TerraformValueAttribute).FullName}");
                    }

                    Visit(new VisitPropertyGenericArgumentContext(context, terraformValueGenericTypeArgument, terraformValueGenericTypeArgumentIndex) { ContextType = TerraformVisitContextType.TerraformValue });
                    return true;
                }

                throw new NotSupportedException($"Custom types implementing {typeof(ITerraformValue<>).FullName} without using {typeof(TerraformValueAttribute).FullName} is not supported");
            }

            return false;
        }

        protected override void VisitPropertyGenericArgument(VisitPropertyGenericArgumentContext context)
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

        public override void VisitComplex(Type complexType, RootVisitContext? context = null)
        {
            if (!complexType.IsBlockAnnotated(out _)) {
                throw new ArgumentException($"Schema type must be annotated with [{typeof(BlockAttribute).FullName}]");
            }

            base.VisitComplex(complexType, context);
        }
    }
}
