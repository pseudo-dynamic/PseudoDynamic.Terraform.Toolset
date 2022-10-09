using System.Reflection;
using Namotion.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.Conventions;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class BlockSchemaBuilder
    {
        public static readonly BlockSchemaBuilder Default = new BlockSchemaBuilder(new AttributeNameConvention(SnakeCaseConvention.Default));

        private BlockNodeBuilder _blockTypeNodeProducer = new BlockNodeBuilder();
        private IAttributeNameConvention _attributeNameConvention;

        public BlockSchemaBuilder(IAttributeNameConvention attributeNameConvention) =>
            _attributeNameConvention = attributeNameConvention ?? throw new ArgumentNullException(nameof(attributeNameConvention));

        protected ValueDefinition BuildList(BlockNode<IVisitPropertySegmentContext> node) =>
            new MonoRangeDefinition(TerraformTypeConstraint.List, BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value);

        protected ValueDefinition BuildSet(BlockNode<IVisitPropertySegmentContext> node) =>
            new MonoRangeDefinition(TerraformTypeConstraint.Set, BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value);

        protected ValueDefinition BuildMap(BlockNode<IVisitPropertySegmentContext> node) =>
            new MapDefinition(BuildValue(node.ElementAt(1).AsContext<IVisitPropertySegmentContext>()).Value);

        private TAttribute ExtendAttribute<TAttribute>(TAttribute attribute, BlockNode<IVisitPropertySegmentContext> node)
        where TAttribute : AttributeDefinition
        {
            var nullability = node.Context.NullabilityInfo.ReadState;
            var isNullable = nullability == NullabilityState.Nullable;
            var isRequired = !isNullable;
            var IsOptional = isNullable;

            return attribute with {
                IsRequired = isRequired,
                IsOptional = IsOptional,
            };
        }

        protected ObjectAttributeDefinition BuildObjectAttribute(BlockNode<IVisitPropertySegmentContext> node)
        {
            var attributeName = _attributeNameConvention.Format(node.Context.Property);
            var valueResult = BuildValue(node.AsContext<IVisitPropertySegmentContext>());
            var attribute = new ObjectAttributeDefinition(attributeName, valueResult.Value);
            return ExtendAttribute(attribute, valueResult.UnwrappedNode);
        }

        protected ObjectDefinition BuildObject(BlockNode node) => new ObjectDefinition() {
            Attributes = node.Select(node => BuildObjectAttribute(node.AsContext<IVisitPropertySegmentContext>())).ToList()
        };

        protected BlockAttributeDefinition BuildBlockAttribute(BlockNode<IVisitPropertySegmentContext> node)
        {
            var propertyContext = node.Context;
            var propertyType = propertyContext.VisitedType;
            var attributeName = _attributeNameConvention.Format(propertyContext.Property);

            var isComputed = propertyType.GetCustomAttribute<ComputedAttribute>(inherit: true) is not null;
            var isSensitive = propertyType.GetCustomAttribute<SensitiveAttribute>(inherit: true) is not null;
            var isDeprecated = propertyType.GetCustomAttribute<DeprecatedAttribute>(inherit: true) is not null;
            var isMarkdownDescription = propertyType.GetCustomAttribute<MarkdownDescription>(inherit: true) is not null;

            var descriptionKind = isMarkdownDescription
                ? DescriptionKind.Markdown
                : DescriptionKind.Plain;

            var description = propertyContext.Property.ToContextualProperty().GetXmlDocsSummary(new XmlDocsOptions() {
                FormattingMode = isMarkdownDescription
                    ? XmlDocsFormattingMode.Markdown
                    : XmlDocsFormattingMode.None
            });

            var valueResult = BuildValue(node.AsContext<IVisitPropertySegmentContext>());

            var attribute = new BlockAttributeDefinition(attributeName, valueResult.Value) {
                IsComputed = isComputed,
                IsSensitive = isSensitive,
                IsDeprecated = isDeprecated,
                Description = description,
                DescriptionKind = descriptionKind
            };

            if (isComputed) {
                return attribute;
            } else {
                return ExtendAttribute(attribute, valueResult.UnwrappedNode);
            }
        }

        protected BlockDefinition BuildBlock(BlockNode node)
        {
            var schemaVersion = node.Context.VisitedType.GetCustomAttribute<SchemaVersionAttribute>()?.SchemaVersion ?? 1;

            return new BlockDefinition() {
                SchemaVersion = schemaVersion,
                Attributes = node.Select(node => BuildBlockAttribute(node.AsContext<IVisitPropertySegmentContext>())).ToList()
            };
        }

        protected ValueResult BuildValue(BlockNode<IVisitPropertySegmentContext> node)
        {
            var isTerraformValue = node.TryUnwrapTerraformValue(out var unwrappedNode);
            var valueTypeConstraint = unwrappedNode.DetermineTypeConstraint();
            ValueDefinition value;

            if (valueTypeConstraint == TerraformTypeConstraint.Block) {
                value = BuildBlock(unwrappedNode);
            } else if (valueTypeConstraint == TerraformTypeConstraint.Object) {
                value = BuildObject(unwrappedNode);
            } else if (valueTypeConstraint == TerraformTypeConstraint.List) {
                value = BuildList(unwrappedNode);
            } else if (valueTypeConstraint == TerraformTypeConstraint.Set) {
                value = BuildSet(unwrappedNode);
            } else if (valueTypeConstraint == TerraformTypeConstraint.Map) {
                value = BuildMap(unwrappedNode);
            } else if (valueTypeConstraint == TerraformTypeConstraint.Tuple) {
                throw new NotImplementedException("A C# Tuple Schema API has not been yet implemented");
            } else {
                value = new PrimitiveDefinition(valueTypeConstraint);
            }

            return new ValueResult(
                value with { IsWrappedByTerraformValue = isTerraformValue },
                unwrappedNode);
        }

        public TerraformDefinition BuildSchema(Type blockType)
        {
            var node = _blockTypeNodeProducer.BuildNode(blockType);
            return BuildBlock(node);
        }

        public TerraformDefinition BuildSchema<T>() =>
            BuildSchema(typeof(T));

        protected class ValueResult
        {
            public ValueDefinition Value { get; }
            public BlockNode<IVisitPropertySegmentContext> UnwrappedNode { get; }

            public ValueResult(ValueDefinition value, BlockNode<IVisitPropertySegmentContext> unwrappedNode)
            {
                Value = value;
                UnwrappedNode = unwrappedNode;
            }
        }
    }
}
