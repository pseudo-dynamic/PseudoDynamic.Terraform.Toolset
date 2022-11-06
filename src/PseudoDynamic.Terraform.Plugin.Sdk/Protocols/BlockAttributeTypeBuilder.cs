using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    /// <summary>
    /// Builder composes the block attribute type field of a schema
    /// attribute message defined in the Terraform plugin protocol.
    /// </summary>
    internal class BlockAttributeTypeBuilder
    {
        public readonly static BlockAttributeTypeBuilder Default = new();

        public string BuildJsonType(TerraformDefinition definition)
        {
            TypeStackingVisitor visitor = new();
            Stack<TerraformDefinition> stack = TerraformDefinitionCollector.Default.Stack(definition);

            while (stack.TryPop(out TerraformDefinition? poppedDefinition)) {
                visitor.Visit(poppedDefinition);
            }

            return visitor.Last;
        }

        private class TypeStackingVisitor : TerraformDefinitionVisitor
        {
            private static List<T> PopRangeReversed<T>(Stack<T> stack, int count)
            {
                List<T> result = new(count);

                while (count-- > 0 && stack.Count > 0) {
                    result.Add(stack.Pop());
                }

                result.Reverse();
                return result;
            }

            public string Last => _jsonBlockTypes.Peek();

            private readonly Stack<string> _jsonBlockTypes = new();

            private void PushBlockType(string blockType) =>
                _jsonBlockTypes.Push(blockType);

            private string PopBlockType() =>
                _jsonBlockTypes.Pop();

            private IEnumerable<string> PopBlockTypesReversed(int count) =>
                PopRangeReversed(_jsonBlockTypes, count);

            protected internal override void VisitDynamic(DynamicDefinition definition) =>
                PushBlockType(definition.TypeConstraint.GetBlockAttributeTypeJsonString());

            protected internal override void VisitPrimitive(PrimitiveDefinition definition) =>
                PushBlockType(definition.TypeConstraint.GetBlockAttributeTypeJsonString());

            private void PopThenPushBlockType(RangeDefinition definition) =>
                PushBlockType($"[{definition.TypeConstraint.GetBlockAttributeTypeJsonString()},{PopBlockType()}]");

            protected internal override void VisitMonoRange(MonoRangeDefinition definition) =>
                PopThenPushBlockType(definition);

            protected internal override void VisitMap(MapDefinition definition) =>
                PopThenPushBlockType(definition);

            protected internal override void VisitObjectAttribute(ObjectAttributeDefinition definition) =>
                PushBlockType($"\"{definition.Name}\":{PopBlockType()}");

            protected internal override void VisitObject(ObjectDefinition definition)
            {
                IEnumerable<string> orderedBlockTypes = PopBlockTypesReversed(definition.Attributes.Count);
                string concatenatedBlockTypes = string.Join(",", orderedBlockTypes);
                PushBlockType($$"""[{{definition.TypeConstraint.GetBlockAttributeTypeJsonString()}},{{{concatenatedBlockTypes}}}]""");
            }

            protected internal override void VisitTuple(TupleDefinition definition) =>
                throw new NotImplementedException();

            protected internal override void VisitBlock(BlockDefinition definition) =>
                throw new NotSupportedException();

            protected internal override void VisitBlockAttribute(BlockAttributeDefinition definition) =>
                throw new NotSupportedException();

            protected internal override void VisitNestedBlock(NestedBlockAttributeDefinition definition) =>
                throw new NotSupportedException();
        }
    }
}
