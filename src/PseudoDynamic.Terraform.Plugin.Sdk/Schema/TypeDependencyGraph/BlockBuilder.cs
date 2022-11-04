using System.Reflection;
using Namotion.Reflection;
using PseudoDynamic.Terraform.Plugin.Conventions;
using PseudoDynamic.Terraform.Plugin.Schema.Conventions;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class BlockBuilder
    {
        public static readonly BlockBuilder Default = new BlockBuilder(new AttributeNameConvention(SnakeCaseConvention.Default));

        private BlockNodeBuilder nodeBuilder = new BlockNodeBuilder();
        private IAttributeNameConvention _attributeNameConvention;

        public BlockBuilder(IAttributeNameConvention attributeNameConvention) =>
            _attributeNameConvention = attributeNameConvention ?? throw new ArgumentNullException(nameof(attributeNameConvention));

        protected DynamicDefinition BuildDynamic(BlockNode<IVisitPropertySegmentContext> node) =>
            new DynamicDefinition(node);

        protected MonoRangeDefinition BuildList(BlockNode<IVisitPropertySegmentContext> node)
        {
            var item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(node.Context.VisitType, TerraformTypeConstraint.List, item);
        }

        protected MonoRangeDefinition BuildSet(BlockNode<IVisitPropertySegmentContext> node)
        {
            var item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(node.Context.VisitType, TerraformTypeConstraint.Set, item);
        }

        protected MapDefinition BuildMap(BlockNode<IVisitPropertySegmentContext> node)
        {
            var value = BuildValue(node.ElementAt(1).AsContext<IVisitPropertySegmentContext>()).Value;
            return new MapDefinition(node.Context.VisitType, value);
        }

        private bool IsAttributeOptional(BlockNode<IVisitPropertySegmentContext> node)
        {
            if (node.Context.Property.GetCustomAttribute<OptionalAttribute>() is not null) {
                return true;
            }

            var nullability = node.Context.NullabilityInfo.ReadState;
            return nullability == NullabilityState.Nullable;
        }

        private AttributeReflectionMetadata CreateAttributeReflectionMetadata(IVisitPropertySegmentContext context) =>
            new AttributeReflectionMetadata(context.Property);

        protected ObjectAttributeDefinition BuildObjectAttribute(BlockNode<IVisitPropertySegmentContext> node)
        {
            var context = node.Context;
            var attributeName = _attributeNameConvention.Format(context.Property);
            var valueResult = BuildValue(node.AsContext<IVisitPropertySegmentContext>());
            var isOptional = IsAttributeOptional(valueResult.UnwrappedNode);
            var reflectionMetadata = CreateAttributeReflectionMetadata(context);

            return new ObjectAttributeDefinition(context.VisitType, attributeName, valueResult.Value) {
                IsOptional = isOptional,
                AttributeReflectionMetadata = reflectionMetadata
            };
        }

        private ComplexReflectionMetadata CreateComplexReflectionMetadata(VisitContext context, IEnumerable<AttributeDefinition> attributes)
        {
            var complexTypeMetadata = context.ComplexTypeMetadata!;
            var propertyNameAttributeNameMapping = attributes.ToDictionary(x => x.AttributeReflectionMetadata.Property.Name, x => x.Name);

            return new ComplexReflectionMetadata(
                complexTypeMetadata,
                propertyNameAttributeNameMapping);
        }

        protected ObjectDefinition BuildObject(BlockNode node)
        {
            var attributes = node.Select(node => BuildObjectAttribute(node.AsContext<IVisitPropertySegmentContext>())).ToList();
            var reflectionMetadata = CreateComplexReflectionMetadata(node.Context, attributes);

            return new ObjectDefinition(node.Context.VisitType) {
                Attributes = attributes,
                ComplexReflectionMetadata = reflectionMetadata
            };
        }

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
            var reflectionMetadata = CreateAttributeReflectionMetadata(context);

            return new BlockAttributeDefinition(context.VisitType, attributeName, valueResult.Value) {
                IsComputed = isComputed,
                IsSensitive = isSensitive,
                IsDeprecated = isDeprecated,
                Description = description,
                DescriptionKind = descriptionKind,
                IsOptional = isOptional,
                AttributeReflectionMetadata = reflectionMetadata
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
            var visitType = node.Context.VisitType;

            var blockAttributeVersion = context.GetContextualAttribute<BlockAttribute>()?.GetVersion();

            if (blockAttributeVersion == null && context.ContextType == VisitContextType.PropertySegment) {
                blockAttributeVersion = context.GetVisitTypeAttribute<BlockAttribute>()?.GetVersion();
            }

            var version = blockAttributeVersion ?? BlockDefinition.DefaultVersion;
            var isDeprecated = context.GetContextualAttribute<DeprecatedAttribute>() is not null;
            var descriptionKind = context.GetContextualAttribute<DescriptionKindAttribute>()?.DescriptionKind ?? default;

            var description = visitType.GetXmlDocsSummary(new XmlDocsOptions() {
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
                .Select(BuildBlockAttribute)
                .ToList();

            var blocks = childNodeValueResults
                .Where(x => x.IsNestedBlock)
                .Select(BuildNestedBlockAttribute)
                .ToList();

            var reflectionMetadata = CreateComplexReflectionMetadata(node.Context, ((IEnumerable<AttributeDefinition>)attributes).Concat(blocks));

            return new BlockDefinition(visitType) {
                Version = version,
                Attributes = attributes,
                Blocks = blocks,
                Description = description,
                DescriptionKind = descriptionKind,
                IsDeprecated = isDeprecated,
                ComplexReflectionMetadata = reflectionMetadata
            };
        }

        protected ValueDefinition BuildValue(BlockNode<IVisitPropertySegmentContext> node, TerraformTypeConstraint valueTypeConstraint) => valueTypeConstraint switch {
            TerraformTypeConstraint.Block => BuildBlock(node),
            TerraformTypeConstraint.Object => BuildObject(node),
            TerraformTypeConstraint.List => BuildList(node),
            TerraformTypeConstraint.Set => BuildSet(node),
            TerraformTypeConstraint.Map => BuildMap(node),
            TerraformTypeConstraint.Tuple => throw new NotImplementedException("A tuple schema API has not been implemented yet"),
            TerraformTypeConstraint.Dynamic => BuildDynamic(node),
            _ => new PrimitiveDefinition(node.Context.VisitType, valueTypeConstraint)
        };

        protected ValueResult BuildValue(BlockNode<IVisitPropertySegmentContext> node)
        {
            var sourceTypeWrapping = node.TryUnwrap(out var unwrappedNode);
            var isWrappedByTerraformValue = sourceTypeWrapping.Contains(TypeWrapping.TerraformValue);

            var explicitTypeConstraint = unwrappedNode.Context.DetermineExplicitTypeConstraint();
            bool isNestedBlock;
            ValueDefinition builtValue;

            // We can now allow to treat dynamic as block
            if (explicitTypeConstraint == TerraformTypeConstraint.Block) {
                var implicitValueTypeConstraints = unwrappedNode.Context.ImplicitTypeConstraints;

                var singleImplicitValueTypeConstraint = node.Context.GetContextualAttribute<NestedBlockAttribute>()?.WrappedBy?.ToTypeConstraint()
                    ?? (implicitValueTypeConstraints.Count == 1
                        ? implicitValueTypeConstraints.Single()
                        : default(TerraformTypeConstraint?));

                if (!singleImplicitValueTypeConstraint.HasValue) {
                    throw new NestedBlockException();
                }

                var unwrappedContext = unwrappedNode.Context;

                if (singleImplicitValueTypeConstraint.Value.IsComplex()) {
                    builtValue = BuildBlock(unwrappedNode);
                } else if (isWrappedByTerraformValue) {
                    throw new NestedBlockException($"The {unwrappedContext.Property.GetPath()} property wants to be a nested block but can only be wrapped by " +
                        $"{TerraformValue.InterfaceGenericTypeDefinition.FullName} if the implicit type constraint is object, tuple or block");
                } else if (singleImplicitValueTypeConstraint.Value.IsRange()) {
                    builtValue = BuildValue(unwrappedNode, singleImplicitValueTypeConstraint.Value);
                } else {
                    throw new NestedBlockException($"The {unwrappedContext.Property.GetPath()} property wants to be a nested block but the property type represents " +
                        $@"either none, unknown or to many implicit Terraform constraint types: {string.Join(", ", implicitValueTypeConstraints)}
Allowed Terraform value: either object, tuple, block, or list, set, map with object or block as direct child
Context type = {unwrappedNode.Context.ContextType}
Property type = {unwrappedNode.Context.VisitType}");
                }

                isNestedBlock = true;
            } else {
                builtValue = BuildValue(unwrappedNode, explicitTypeConstraint);
                isNestedBlock = false;
            }

            var updatedValue = builtValue with {
                OuterType = node.Context.VisitType,
                SourceTypeWrapping = sourceTypeWrapping
            };

            return new ValueResult(updatedValue, unwrappedNode) {
                IsNestedBlock = isNestedBlock
            };
        }

        public ValueDefinition BuildDynamic(DynamicDefinition definition, Type knownType)
        {
            var newContext = VisitPropertyGenericSegmentContext.Custom(definition.DynamicNode.Context, knownType);
            var newNode = nodeBuilder.BuildDynamic(newContext).AsContext<IVisitPropertySegmentContext>();
            return BuildValue(newNode).Value;
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
