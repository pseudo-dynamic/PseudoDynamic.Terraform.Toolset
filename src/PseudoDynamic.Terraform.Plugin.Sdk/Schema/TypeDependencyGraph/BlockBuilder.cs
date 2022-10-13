using System.Reflection;
using Namotion.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.Conventions;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Block;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal class BlockBuilder
    {
        public static readonly BlockBuilder Default = new BlockBuilder(new AttributeNameConvention(SnakeCaseConvention.Default));

        private BlockNodeBuilder nodeBuilder = new BlockNodeBuilder();
        private IAttributeNameConvention _attributeNameConvention;

        public BlockBuilder(IAttributeNameConvention attributeNameConvention) =>
            _attributeNameConvention = attributeNameConvention ?? throw new ArgumentNullException(nameof(attributeNameConvention));

        protected ValueDefinition BuildList(BlockNode<IVisitPropertySegmentContext> node)
        {
            var item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(TerraformTypeConstraint.List, item);
        }

        protected ValueDefinition BuildSet(BlockNode<IVisitPropertySegmentContext> node)
        {
            var item = BuildValue(node.Single().AsContext<IVisitPropertySegmentContext>()).Value;
            return new MonoRangeDefinition(TerraformTypeConstraint.Set, item);
        }

        protected ValueDefinition BuildMap(BlockNode<IVisitPropertySegmentContext> node)
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

        protected BlockAttributeDefinition BuildBlockAttribute(BlockNode<IVisitPropertySegmentContext> node)
        {
            var propertyContext = node.Context;
            var property = propertyContext.Property;
            var attributeName = _attributeNameConvention.Format(propertyContext.Property);

            var isComputed = property.GetCustomAttribute<ComputedAttribute>(inherit: true) is not null;
            var isSensitive = property.GetCustomAttribute<SensitiveAttribute>(inherit: true) is not null;
            var isDeprecated = property.GetCustomAttribute<DeprecatedAttribute>(inherit: true) is not null;
            var descriptionKind = property.GetCustomAttribute<DescriptionKindAttribute>(inherit: true)?.DescriptionKind ?? default;

            var description = propertyContext.Property.GetXmlDocsSummary(new XmlDocsOptions() {
                FormattingMode = descriptionKind == DescriptionKind.Markdown
                    ? XmlDocsFormattingMode.Markdown
                    : XmlDocsFormattingMode.None
            });

            var valueResult = BuildValue(node.AsContext<IVisitPropertySegmentContext>());
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

        protected BlockDefinition BuildBlock(BlockNode node)
        {
            var visitedType = node.Context.VisitedType;
            var schemaVersion = visitedType.GetCustomAttribute<SchemaVersionAttribute>()?.SchemaVersion ?? BlockDefinition.DefaultSchemaVersion;
            var isDeprecated = visitedType.GetCustomAttribute<DeprecatedAttribute>(inherit: true) is not null;
            var descriptionKind = visitedType.GetCustomAttribute<DescriptionKindAttribute>(inherit: true)?.DescriptionKind ?? default;

            var description = visitedType.GetXmlDocsSummary(new XmlDocsOptions() {
                FormattingMode = descriptionKind == DescriptionKind.Markdown
                    ? XmlDocsFormattingMode.Markdown
                    : XmlDocsFormattingMode.None
            });

            return new BlockDefinition() {
                SchemaVersion = schemaVersion,
                Attributes = node.Select(node => BuildBlockAttribute(node.AsContext<IVisitPropertySegmentContext>())).ToList(),
                Description = description,
                DescriptionKind = descriptionKind,
                IsDeprecated = isDeprecated
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
                throw new NotImplementedException("A tuple schema API has not been implemented yet");
            } else {
                value = new PrimitiveDefinition(valueTypeConstraint);
            }

            return new ValueResult(
                value with { IsWrappedByTerraformValue = isTerraformValue },
                unwrappedNode);
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

            public ValueResult(ValueDefinition value, BlockNode<IVisitPropertySegmentContext> unwrappedNode)
            {
                Value = value;
                UnwrappedNode = unwrappedNode;
            }
        }
    }
}
