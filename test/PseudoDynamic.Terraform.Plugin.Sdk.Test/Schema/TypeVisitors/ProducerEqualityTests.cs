using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;
using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors;
using PseudoDynamic.Terraform.Plugin.Internals;

namespace PseudoDynamic.Terraform.Plugin.Schema.Visitors
{
    public class ProducerEqualityTests
    {
        public static IEnumerable<object[]> GetBlockTypeNodes()
        {
            yield return new object[] {
                typeof(ZeroDepthBlock),
                new BlockTypeNode(new VisitContext(typeof(ZeroDepthBlock)) { ContextType = VisitContextType.Complex })
            };

            yield return new object[] {
                typeof(PropertyBlock),
                new BlockTypeNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockTypeNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                }
            };

            yield return new object[] {
                typeof(NestedBlock),
                new BlockTypeNode(new VisitContext(typeof(NestedBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockTypeNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.Property }) {
                        new BlockTypeNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.Complex }) {
                            new BlockTypeNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                        }
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentBlock),
                new BlockTypeNode(new VisitContext(typeof(PropertyArgumentBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockTypeNode(new VisitContext(typeof(List<string>)) { ContextType = VisitContextType.Property }) {
                        new BlockTypeNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.PropertyGenericArgument })
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentNestedBlock),
                new BlockTypeNode(new VisitContext(typeof(PropertyArgumentNestedBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockTypeNode(new VisitContext(typeof(IDictionary<string, PropertyBlock>)) { ContextType = VisitContextType.Property }) {
                        new BlockTypeNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.PropertyGenericArgument }),
                        new BlockTypeNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.PropertyGenericArgument }) {
                            new BlockTypeNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.Complex }) {
                                new BlockTypeNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                            }
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
        public class PropertyBlock
        {
            public string String { get; set; }
        }

        [Block]
        public class NestedBlock
        {
            public PropertyBlock Schema { get; set; }
        }

        [Block]
        public class PropertyArgumentBlock
        {
            public List<string> List { get; set; }
        }

        [Block]
        public class PropertyArgumentNestedBlock
        {
            public IDictionary<string, PropertyBlock> Dictionary { get; set; }
        }
    }
}
