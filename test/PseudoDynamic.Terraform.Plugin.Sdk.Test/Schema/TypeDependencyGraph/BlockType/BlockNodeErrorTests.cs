using FluentAssertions;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    public class BlockNodeErrorTests
    {
        [Fact]
        internal void Node_builder_throws_because_of_missing_complex_attribute_annotation()
        {
            MissingAttributeAnnotationException error = BlockNodeBuilder.Default.Invoking(x => x.BuildNode<ListOfObjects>())
                .Should().Throw<MissingAttributeAnnotationException>()
                .And;

            error.ReceiverType.Should().Be(typeof(ListOfObjects.MissingObjectAnnotatedObject));
            error.MissingAttributeType.Should().Be(typeof(ComplexAttribute));
        }

        [Block]
        public class ListOfObjects
        {
            public IList<MissingObjectAnnotatedObject> List { get; set; } = null!;

            public class MissingObjectAnnotatedObject
            {
                public string String { get; set; } = null!;
            }
        }
    }
}
