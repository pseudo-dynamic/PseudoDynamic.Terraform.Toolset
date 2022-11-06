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
        public static readonly BlockBuilder Default = new(new AttributeNameConvention(SnakeCaseConvention.Default));

        private readonly BlockNodeBuilder _nodeBuilder = new();
        private readonly IAttributeNameConvention _attributeNameConvention;

        public BlockBuilder(IAttributeNameConvention attributeNameConvention) =>
            _attributeNameConvention = attributeNameConvention ?? throw new ArgumentNullException(nameof(attributeNameConvention));

        protected DynamicDefinition BuildDynamic(BlockNode<IVisitPropertySegmentContext> node) =>
            new(node);

        protected MonoRangeDefinition BuildList(BlockNode<IVisitPropertySegmentContext> node)
        {
            ValueDefinition item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(node.Context.VisitType, TerraformTypeConstraint.List, item);
        }

        protected MonoRangeDefinition BuildSet(BlockNode<IVisitPropertySegmentContext> node)
        {
            ValueDefinition item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(node.Context.VisitType, TerraformTypeConstraint.Set, item);
        }

        protected MapDefinition BuildMap(BlockNode<IVisitPropertySegmentContext> node)
        {
            ValueDefinition value = BuildValue(node.ElementAt(1).AsContext<IVisitPropertySegmentContext>()).Value;
            return new MapDefinition(node.Context.VisitType, value);
        }

        private bool IsAttributeOptional(BlockNode<IVisitPropertySegmentContext> node)
        {
            if (node.Context.Property.GetCustomAttribute<OptionalAttribute>() is not null) {
                return true;
            }

            NullabilityState nullability = node.Context.NullabilityInfo.ReadState;
            return nullability == NullabilityState.Nullable;
        }

        private AttributeReflectionMetadata CreateAttributeReflectionMetadata(IVisitPropertySegmentContext context) =>
            new(context.Property);

        protected ObjectAttributeDefinition BuildObjectAttribute(BlockNode<IVisitPropertySegmentContext> node)
        {
            IVisitPropertySegmentContext context = node.Context;
            string attributeName = _attributeNameConvention.Format(context.Property);
            ValueResult valueResult = BuildValue(node.AsContext<IVisitPropertySegmentContext>());
            bool isOptional = IsAttributeOptional(valueResult.UnwrappedNode);
            AttributeReflectionMetadata reflectionMetadata = CreateAttributeReflectionMetadata(context);

            return new ObjectAttributeDefinition(context.VisitType, attributeName, valueResult.Value) {
                IsOptional = isOptional,
                AttributeReflectionMetadata = reflectionMetadata
            };
        }

        private ComplexReflectionMetadata CreateComplexReflectionMetadata(VisitContext context, IEnumerable<AttributeDefinition> attributes)
        {
            ComplexTypeMetadata complexTypeMetadata = context.ComplexTypeMetadata!;
            Dictionary<string, string> propertyNameAttributeNameMapping = attributes.ToDictionary(x => x.AttributeReflectionMetadata.Property.Name, x => x.Name);

            return new ComplexReflectionMetadata(
                complexTypeMetadata,
                propertyNameAttributeNameMapping);
        }

        protected ObjectDefinition BuildObject(BlockNode node)
        {
            List<ObjectAttributeDefinition> attributes = node.Select(node => BuildObjectAttribute(node.AsContext<IVisitPropertySegmentContext>())).ToList();
            ComplexReflectionMetadata reflectionMetadata = CreateComplexReflectionMetadata(node.Context, attributes);

            return new ObjectDefinition(node.Context.VisitType) {
                Attributes = attributes,
                ComplexReflectionMetadata = reflectionMetadata
            };
        }

        protected BlockAttributeDefinition BuildBlockAttribute(ValueResult valueResult)
        {
            IVisitPropertySegmentContext context = valueResult.UnwrappedNode.Context;
            PropertyInfo property = context.Property;
            string attributeName = _attributeNameConvention.Format(property);

            bool isComputed = context.GetContextualAttribute<ComputedAttribute>() is not null;
            bool isSensitive = context.GetContextualAttribute<SensitiveAttribute>() is not null;
            bool isDeprecated = context.GetContextualAttribute<DeprecatedAttribute>() is not null;
            DescriptionKind descriptionKind = context.GetContextualAttribute<DescriptionKindAttribute>()?.DescriptionKind ?? default;

            string description = property.GetXmlDocsSummary(new XmlDocsOptions() {
                FormattingMode = descriptionKind == DescriptionKind.Markdown
                    ? XmlDocsFormattingMode.Markdown
                    : XmlDocsFormattingMode.None
            });

            bool isOptional = IsAttributeOptional(valueResult.UnwrappedNode);
            AttributeReflectionMetadata reflectionMetadata = CreateAttributeReflectionMetadata(context);

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
            BlockAttributeDefinition blockAttribute = BuildBlockAttribute(valueResult);
            NestedBlockAttribute? nestedBlockAttribute = valueResult.UnwrappedNode.Context.GetContextualAttribute<NestedBlockAttribute>();
            int minimumItems = nestedBlockAttribute?.MinimumItems ?? NestedBlockAttributeDefinition.DefaultMinimumItems;
            int maximumItems = nestedBlockAttribute?.MaximumItems ?? NestedBlockAttributeDefinition.DefaultMaximumItems;

            return new NestedBlockAttributeDefinition(blockAttribute) {
                MinimumItems = minimumItems,
                MaximumItems = maximumItems
            };
        }

        protected BlockDefinition BuildBlock(BlockNode node)
        {
            VisitContext context = node.Context;
            Type visitType = node.Context.VisitType;

            int? blockAttributeVersion = context.GetContextualAttribute<BlockAttribute>()?.GetVersion();

            if (blockAttributeVersion == null && context.ContextType == VisitContextType.PropertySegment) {
                blockAttributeVersion = context.GetVisitTypeAttribute<BlockAttribute>()?.GetVersion();
            }

            int version = blockAttributeVersion ?? BlockDefinition.DefaultVersion;
            bool isDeprecated = context.GetContextualAttribute<DeprecatedAttribute>() is not null;
            DescriptionKind descriptionKind = context.GetContextualAttribute<DescriptionKindAttribute>()?.DescriptionKind ?? default;

            string description = visitType.GetXmlDocsSummary(new XmlDocsOptions() {
                FormattingMode = descriptionKind == DescriptionKind.Markdown
                    ? XmlDocsFormattingMode.Markdown
                    : XmlDocsFormattingMode.None
            });

            List<BlockNode<IVisitPropertySegmentContext>> childNodes = node
                .Select(x => x.AsContext<IVisitPropertySegmentContext>())
                .ToList();

            List<ValueResult> childNodeValueResults = childNodes.ConvertAll(BuildValue)
;

            List<BlockAttributeDefinition> attributes = childNodeValueResults
                .Where(x => !x.IsNestedBlock)
                .Select(BuildBlockAttribute)
                .ToList();

            List<NestedBlockAttributeDefinition> blocks = childNodeValueResults
                .Where(x => x.IsNestedBlock)
                .Select(BuildNestedBlockAttribute)
                .ToList();

            ComplexReflectionMetadata reflectionMetadata = CreateComplexReflectionMetadata(node.Context, ((IEnumerable<AttributeDefinition>)attributes).Concat(blocks));

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
            IReadOnlyList<TypeWrapping> sourceTypeWrapping = node.TryUnwrap(out BlockNode<IVisitPropertySegmentContext>? unwrappedNode);
            bool isWrappedByTerraformValue = sourceTypeWrapping.Contains(TypeWrapping.TerraformValue);

            TerraformTypeConstraint explicitTypeConstraint = unwrappedNode.Context.DetermineExplicitTypeConstraint();
            bool isNestedBlock;
            ValueDefinition builtValue;

            // We can now allow to treat dynamic as block
            if (explicitTypeConstraint == TerraformTypeConstraint.Block) {
                IReadOnlySet<TerraformTypeConstraint> implicitValueTypeConstraints = unwrappedNode.Context.ImplicitTypeConstraints;

                TerraformTypeConstraint? singleImplicitValueTypeConstraint = node.Context.GetContextualAttribute<NestedBlockAttribute>()?.WrappedBy?.ToTypeConstraint()
                    ?? (implicitValueTypeConstraints.Count == 1
                        ? implicitValueTypeConstraints.Single()
                        : default(TerraformTypeConstraint?));

                if (!singleImplicitValueTypeConstraint.HasValue) {
                    throw new NestedBlockException();
                }

                IVisitPropertySegmentContext unwrappedContext = unwrappedNode.Context;

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

            ValueDefinition updatedValue = builtValue with {
                OuterType = node.Context.VisitType,
                SourceTypeWrapping = sourceTypeWrapping
            };

            return new ValueResult(updatedValue, unwrappedNode) {
                IsNestedBlock = isNestedBlock
            };
        }

        public ValueDefinition BuildDynamic(DynamicDefinition definition, Type knownType)
        {
            VisitPropertyGenericSegmentContext newContext = VisitPropertyGenericSegmentContext.Custom(definition.DynamicNode.Context, knownType);
            BlockNode<IVisitPropertySegmentContext> newNode = _nodeBuilder.BuildDynamic(newContext).AsContext<IVisitPropertySegmentContext>();
            return BuildValue(newNode).Value;
        }

        public BlockDefinition BuildBlock(Type blockType)
        {
            BlockNode node = _nodeBuilder.BuildNode(blockType);
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
