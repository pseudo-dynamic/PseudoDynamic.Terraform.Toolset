using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    public class BlockNodeTerraformValueEqualityTests
    {
        public static IEnumerable<object[]> GetBlockTypeNodes()
        {
            yield return new object[] {
                typeof(TerraformValueBlock),
                new BlockNode(new VisitContext(typeof(TerraformValueBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(TerraformValue<ZeroDepthBlock>)) { ContextType = VisitContextType.Property }) {
                        new BlockNode(new VisitContext(typeof(ZeroDepthBlock)) { ContextType = TerraformVisitContextType.TerraformValue})
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetBlockTypeNodes))]
        internal void Node_producer_produces_correct_blocks(Type schemaType, BlockNode expectedNode)
        {
            var actualNode = new BlockNodeBuilder().BuildNode(schemaType);
            Assert.Equal(expectedNode, actualNode, BlockNodeContextEqualityComparer.Default);
        }

        [Block]
        public class ZeroDepthBlock { }

        [Block]
        public class TerraformValueBlock
        {
            public TerraformValue<ZeroDepthBlock> Block { get; set; }
        }
    }
}
