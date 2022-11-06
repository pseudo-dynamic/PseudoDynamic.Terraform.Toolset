using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    public class BlockNodeNullableEqualityTests
    {
        public static IEnumerable<object[]> Generate_block_nodes()
        {
            yield return new object[] {
                typeof(Block),
                new BlockNode(new VisitContext(typeof(Block)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(int?)) { ContextType = VisitContextType.Property }) {
                        new BlockNode(new VisitContext(typeof(int)) { ContextType = BlockVisitContextType.Nullable })
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(Generate_block_nodes))]
        internal void Block_builder_builds_nullable_types(Type schemaType, BlockNode expectedNode)
        {
            var actualNode = new BlockNodeBuilder().BuildNode(schemaType);
            Assert.Equal(expectedNode, actualNode, BlockNodeContextEqualityComparer.Default);
        }

        [Block]
        private class Block
        {
            public int? NullableNumber { get; set; }
        }
    }
}
