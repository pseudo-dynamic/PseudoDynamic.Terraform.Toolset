using System.Reflection;
using Namotion.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.Conventions;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class BlockBuilder
    {
        public static readonly BlockBuilder Default = new BlockBuilder(new AttributeNameConvention(SnakeCaseConvention.Default));

        private BlockNodeBuilder nodeBuilder = new BlockNodeBuilder();
        private IAttributeNameConvention _attributeNameConvention;

        public BlockBuilder(IAttributeNameConvention attributeNameConvention) =>
            _attributeNameConvention = attributeNameConvention ?? throw new ArgumentNullException(nameof(attributeNameConvention));

        protected MonoRangeDefinition BuildList(BlockNode<IVisitPropertySegmentContext> node)
        {
            var item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(TerraformTypeConstraint.List, item);
        }

        protected MonoRangeDefinition BuildSet(BlockNode<IVisitPropertySegmentContext> node)
        {
            var item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(TerraformTypeConstraint.Set, item);
        }

        protected MapDefinition BuildMap(BlockNode<IVisitPropertySegmentContext> node)
        {
            var value = BuildValue(node.ElementAt(1).AsContext<IVisitPropertySegmentContext>()).Value;
            return new MapDefinition(value);
        }

        private bool IsAttributeOptional(BlockNode<IVisitPropertySegmentContext> node)
        {
            if (node.Context.Property.GetCustomAttribute<OptionalAttribute>() is not null) {
                return true;
            }

            var nullability = node.Context.NullabilityInfo.ReadState;
            return nullability == NullabilityState.Nullable;
        }

        protected ObjectAttributeDefinition BuildObjectAttribute(BlockNode<IVisitPropertySegmentContext> node)
        {
            var attributeName = _attributeNameConvention.Format(node.Context.Property);
            var valueResult = BuildValue(node.AsContext<IVisitPropertySegmentContext>());
            var isOptional = IsAttributeOptional(valueResult.UnwrappedNode);

            return new ObjectAttributeDefinition(attributeName, valueResult.Value) {
                IsOptional = isOptional
            };
        }

        protected ObjectDefinition BuildObject(BlockNode node) => new ObjectDefinition() {
            Attributes = node.Select(node => BuildObjectAttribute(node.AsContext<IVisitPropertySegmentContext>())).ToList()
        };

        protected BlockAttributeDefinition BuildBlockAttribute(ValueResult valueResult)
        {
            var context = valueResult.UnwrappedNode.Context;
            var property = context.Property;
            var attributeName = _attributeNameConvention.Format(property);

            var isComputed = context.GetContextualAttribute<ComputedAttribute>() is not null;
            var isSensitive = context.GetContextualAttribute<SensitiveAttribute>() is not null;
            var isDeprecated = context.GetContextualAttribute<DeprecatedAttribute>() is not null;
            var descriptionKind = context.GetContextualAttribute<DescriptionKindAttribute>()?.DescriptionKind ?? default;

            var description = property.GetXmlDocsSummary(new XmlDocsOptions() {
                FormattingMode = descriptionKind == DescriptionKind.Markdown
                    ? XmlDocsFormattingMode.Markdown
                    : XmlDocsFormattingMode.None
            });

            var isOptional = IsAttributeOptional(valueResult.UnwrappedNode);

            return new BlockAttributeDefinition(attributeName, valueResult.Value) {
                IsComputed = isComputed,
                IsSensitive = isSensitive,
                IsDeprecated = isDeprecated,
                Description = description,
                DescriptionKind = descriptionKind,
                IsOptional = isOptional
            };
        }

        protected NestedBlockAttributeDefinition BuildNestedBlockAttribute(ValueResult valueResult)
        {
            var blockAttribute = BuildBlockAttribute(valueResult);
            var nestedBlockAttribute = valueResult.UnwrappedNode.Context.GetContextualAttribute<NestedBlockAttribute>();
            var minimumItems = nestedBlockAttribute?.MinimumItems ?? NestedBlockAttributeDefinition.DefaultMinimumItems;
            var maximumItems = nestedBlockAttribute?.MaximumItems ?? NestedBlockAttributeDefinition.DefaultMaximumItems;

            return new NestedBlockAttributeDefinition(blockAttribute) {
                MinimumItems = minimumItems,
                MaximumItems = maximumItems
            };
        }

        protected BlockDefinition BuildBlock(BlockNode node)
        {
            var context = node.Context;
            var visitedType = node.Context.VisitedType;

            var blockAttributeVersion = context.GetContextualAttribute<BlockAttribute>()?.GetVersion();

            if (blockAttributeVersion == null && context.ContextType == VisitContextType.PropertySegment) {
                blockAttributeVersion = context.GetVisitedTypeAttribute<BlockAttribute>()?.GetVersion();
            }

            var version = blockAttributeVersion ?? BlockDefinition.DefaultVersion;
            var isDeprecated = context.GetContextualAttribute<DeprecatedAttribute>() is not null;
            var descriptionKind = context.GetContextualAttribute<DescriptionKindAttribute>()?.DescriptionKind ?? default;

            var description = visitedType.GetXmlDocsSummary(new XmlDocsOptions() {
                FormattingMode = descriptionKind == DescriptionKind.Markdown
                    ? XmlDocsFormattingMode.Markdown
                    : XmlDocsFormattingMode.None
            });

            var childNodes = node
                .Select(x => x.AsContext<IVisitPropertySegmentContext>())
                .ToList();

            var childNodeValueResults = childNodes
                .Select(BuildValue)
                .ToList();

            var attributes = childNodeValueResults
                .Where(x => !x.IsNestedBlock)
                .Select(x => BuildBlockAttribute(x))
                .ToList();

            var blocks = childNodeValueResults
                .Where(x => x.IsNestedBlock)
                .Select(x => BuildNestedBlockAttribute(x))
                .ToList();

            return new BlockDefinition() {
                Version = version,
                Attributes = attributes,
                Blocks = blocks,
                Description = description,
                DescriptionKind = descriptionKind,
                IsDeprecated = isDeprecated
            };
        }

        protected ValueDefinition BuildValue(BlockNode<IVisitPropertySegmentContext> node, TerraformTypeConstraint valueTypeConstraint) => valueTypeConstraint switch {
            TerraformTypeConstraint.Block => BuildBlock(node),
            TerraformTypeConstraint.Object => BuildObject(node),
            TerraformTypeConstraint.List => BuildList(node),
            TerraformTypeConstraint.Set => BuildSet(node),
            TerraformTypeConstraint.Map => BuildMap(node),
            TerraformTypeConstraint.Tuple => throw new NotImplementedException("A tuple schema API has not been implemented yet"),
            _ => new PrimitiveDefinition(valueTypeConstraint)
        };

        protected ValueResult BuildValue(BlockNode<IVisitPropertySegmentContext> node)
        {
            var isTerraformValue = node.TryUnwrapTerraformValue(out var unwrappedNode);
            var unwrappedContext = unwrappedNode.Context;
            var explicitTypeConstraint = unwrappedNode.Context.DetermineExplicitTypeConstraint(out var implicitValueTypeConstraints);
            bool isNestedBlock;
            ValueDefinition builtValue;

            if (explicitTypeConstraint == TerraformTypeConstraint.Block) {
                TerraformTypeConstraint? singleImplicitValueTypeConstraints = node.Context.GetContextualAttribute<NestedBlockAttribute>()?.WrappedBy?.ToTypeConstraint()
                    ?? (implicitValueTypeConstraints.Count == 1
                        ? implicitValueTypeConstraints.Single()
                        : default);

                if (!singleImplicitValueTypeConstraints.HasValue) {
                    throw new NestedBlockException();
                }

                if (singleImplicitValueTypeConstraints.Value.IsBlockLike()) {
                    builtValue = BuildBlock(unwrappedNode);
                } else if (isTerraformValue) {
                    throw new NestedBlockException($"The {unwrappedContext.PropertyPath} property wants to be a nested block but can only be wrapped by " +
                        $"{typeof(ITerraformValue<>).FullName} if the implicit type constraint is object, tuple or block");
                } else if (singleImplicitValueTypeConstraints.Value.IsRange()) {
                    builtValue = BuildValue(unwrappedNode, singleImplicitValueTypeConstraints.Value);
                } else {
                    throw new NestedBlockException($"The {unwrappedContext.PropertyPath} property wants to be a nested block but the property type " +
                        $"can be implictly object, tuple, block, or list, set or map, that contains implictly object, tuple or block");
                }

                isNestedBlock = true;
            } else {
                builtValue = BuildValue(unwrappedNode, explicitTypeConstraint);
                isNestedBlock = false;
            }

            var updatedValue = builtValue with { IsWrappedByTerraformValue = isTerraformValue };

            return new ValueResult(updatedValue, unwrappedNode) {
                IsNestedBlock = isNestedBlock
            };
        }

        public BlockDefinition BuildBlock(Type blockType)
        {
            var node = nodeBuilder.BuildNode(blockType);
            return BuildBlock(node);
        }

        public BlockDefinition BuildBlock<T>() =>
            BuildBlock(typeof(T));

        protected class ValueResult
        {
            public ValueDefinition Value { get; }
            public BlockNode<IVisitPropertySegmentContext> UnwrappedNode { get; }
            public bool IsNestedBlock { get; init; }

            public ValueResult(ValueDefinition value, BlockNode<IVisitPropertySegmentContext> unwrappedNode)
            {
                Value = value;
                UnwrappedNode = unwrappedNode;
            }
        }
    }
}
