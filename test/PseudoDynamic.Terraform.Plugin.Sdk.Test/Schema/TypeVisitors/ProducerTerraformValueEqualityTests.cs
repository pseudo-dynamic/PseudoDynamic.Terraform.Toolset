using PseudoDynamic.Terraform.Plugin.Internals;
using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;
using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    public class ProducerTerraformValueEqualityTests
    {
        public static IEnumerable<object[]> GetBlockTypeNodes()
        {
            yield return new object[] {
                typeof(TerraformValueBlock),
                new BlockTypeNode(new VisitContext(typeof(TerraformValueBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockTypeNode(new VisitContext(typeof(TerraformValue<ZeroDepthBlock>)) { ContextType = VisitContextType.Property }) {
                        new BlockTypeNode(new VisitContext(typeof(ZeroDepthBlock)) { ContextType = TerraformVisitContextType.TerraformValue}) {
                            new BlockTypeNode(new VisitContext(typeof(ZeroDepthBlock)) { ContextType = VisitContextType.Complex})
                        }
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetBlockTypeNodes))]
        internal void Node_producer_produces_correct_blocks(Type schemaType, BlockTypeNode expectedNode)
        {
            var actualNode = new BlockTypeNodeProducer().Produce(schemaType);
            Assert.Equal(expectedNode, actualNode, BlockTypeNodeEqualityComparer<BlockTypeNode>.Default);
        }

        [Block]
        public class ZeroDepthBlock { }

        [Block]
        public class TerraformValueBlock
        {
            public TerraformValue<ZeroDepthBlock?> Block { get; set; }
        }
    }
}
