using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    internal class SchemaBuilder
    {
        private List<TerraformDefinition>? _preloadableDefinitions;
        private TerraformDefinition? _loadableDefinition;
        private int _alreadyPreloaded;
        private ConcurrentDictionary<(TerraformTypeConstraint TypeConstraint, Type SourceType), ValueDefinition> _knownDefinitions = new();
        private ValueDefinitionCollectingVisitor _visitor;

        public SchemaBuilder() => _visitor = new ValueDefinitionCollectingVisitor(this);

        private void ReplaceKnownDefinition(ValueDefinition value) => _knownDefinitions[(value.TypeConstraint, value.SourceType)] = value;

        private void AddPreloadable(TerraformDefinition resource)
        {
            if (_alreadyPreloaded != 0) {
                throw new NotSupportedException();
            }

            _preloadableDefinitions ??= new List<TerraformDefinition>();
            _preloadableDefinitions.Add(resource);
        }

        private void PreloadOnce()
        {
            if (Interlocked.CompareExchange(ref _alreadyPreloaded, 1, 0) == 0) {
                var unprocessedDefinitions = Interlocked.Exchange(ref _preloadableDefinitions, null);

                if (unprocessedDefinitions != null) {
                    unprocessedDefinitions.AsParallel().ForAll(ComputeDynamicDefinitions);
                }
            }
        }

        private void Preload()
        {
            PreloadOnce();

            var unprocessedDefinition = Interlocked.Exchange(ref _loadableDefinition, null);

            if (unprocessedDefinition != null) {
                ComputeDynamicDefinitions(unprocessedDefinition);
            }
        }

        private void ComputeDynamicDefinitions(TerraformDefinition terraformDefinition) =>
            _visitor.Visit(terraformDefinition);

        private bool ResolveCache(Type knownType, [NotNullWhen(true)] out ValueDefinition? value)
        {
            var implicitTypeConstraints = TerraformTypeConstraintEvaluator.Default.Evaluate(knownType);

            if (implicitTypeConstraints.Count == 1 && _knownDefinitions.TryGetValue((implicitTypeConstraints.Single(), knownType), out value)) {
                return true;
            }

            value = null;
            return false;
        }

        internal ValueDefinition BuildDynamic(DynamicDefinition dynamic, Type knownType)
        {
            Preload();

            if (ResolveCache(knownType, out var cache)) {
                return cache;
            }

            var resolvedDefinition = BlockBuilder.Default.BuildDynamic(dynamic, knownType);
            _loadableDefinition = resolvedDefinition;
            ReplaceKnownDefinition(resolvedDefinition);
            return resolvedDefinition;
        }

        internal BlockDefinition BuildBlock(Type schemaType)
        {
            var schema = BlockBuilder.Default.BuildBlock(schemaType);
            AddPreloadable(schema);
            return schema;
        }

        private class ValueDefinitionCollectingVisitor : TerraformDefinitionVisitor
        {
            private SchemaBuilder _registry;

            public ValueDefinitionCollectingVisitor(SchemaBuilder registry) =>
                _registry = registry;

            protected internal override void VisitBlock(BlockDefinition definition)
            {
                _registry.ReplaceKnownDefinition(definition);
                base.VisitBlock(definition);
            }

            protected internal override void VisitMap(MapDefinition definition)
            {
                _registry.ReplaceKnownDefinition(definition);
                base.VisitMap(definition);
            }

            protected internal override void VisitMonoRange(MonoRangeDefinition definition)
            {
                _registry.ReplaceKnownDefinition(definition);
                base.VisitMonoRange(definition);
            }

            protected internal override void VisitObject(ObjectDefinition definition)
            {
                _registry.ReplaceKnownDefinition(definition);
                base.VisitObject(definition);
            }

            protected internal override void VisitPrimitive(PrimitiveDefinition definition)
            {
                _registry.ReplaceKnownDefinition(definition);
                base.VisitPrimitive(definition);
            }

            protected internal override void VisitTuple(TupleDefinition definition)
            {
                _registry.ReplaceKnownDefinition(definition);
                base.VisitTuple(definition);
            }
        }
    }
}
