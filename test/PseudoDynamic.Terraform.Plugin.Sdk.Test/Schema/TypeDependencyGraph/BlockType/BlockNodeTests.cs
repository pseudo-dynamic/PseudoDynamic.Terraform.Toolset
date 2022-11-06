using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    public class BlockSchemaTests
    {
        [Theory]
        [InlineData(new object[] { typeof(NonAnnotatedBlocks.Default) })]
        public void Node_producer_throws_exception_due_to_missing_block_attribute(Type schemaType)
        {
            ArgumentException error = Assert.IsType<ArgumentException>(Record.Exception(() => new BlockNodeBuilder().BuildNode(schemaType)));
            Assert.Contains("must be annotated", error.Message, StringComparison.InvariantCulture);
        }

        [Fact]
        public void Node_producer_throws_exception_due_to_open_generic_type()
        {
            ArgumentException error = Assert.IsType<ArgumentException>(Record.Exception(() => new BlockNodeBuilder().BuildNode(typeof(GenericSchema<>))));
            Assert.Contains("closed generic type", error.Message, StringComparison.InvariantCulture);
        }

        [Theory]
        [InlineData(new object[] { typeof(DependencyCycleBlocks.Property) })]
        [InlineData(new object[] { typeof(DependencyCycleBlocks.PropertyArgument) })]
        [InlineData(new object[] { typeof(DependencyCycleBlocks.Generic<object>) })]
        public void Node_producer_detects_dependency_cycle(Type schemaType)
        {
            _ = Assert.IsType<TypeDependencyCycleException>(Record.Exception(() => new BlockNodeBuilder().BuildNode(schemaType)));
        }

        public class NonAnnotatedBlocks
        {
            public class Default { }

            [Block]
            public class Property
            {
                public NestedSchema Infinite { get; set; } = null!;

                public class NestedSchema { }
            }
        }

        [Block]
        public class GenericSchema<T> { }

        public static class DependencyCycleBlocks
        {
            [Block]
            public class Property
            {
                public Property Infinite { get; set; } = null!;
            }

            [Block]
            public class PropertyArgument
            {
                public IList<PropertyArgument> Infinite { get; set; } = null!;
            }

            [Block]
            public class Generic<T>
            {
                public IList<Generic<T>> Infinite { get; set; } = null!;
            }
        }
    }
}
