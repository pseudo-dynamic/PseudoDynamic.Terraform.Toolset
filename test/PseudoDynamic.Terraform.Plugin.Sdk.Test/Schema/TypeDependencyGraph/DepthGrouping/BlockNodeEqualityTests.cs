using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping
{
    public class BlockNodeEqualityTests
    {
        public static IEnumerable<object[]> GetBlockTypeNodes()
        {
            yield return new object[] {
                typeof(ZeroDepthBlock),
                new BlockNode(new VisitContext(typeof(ZeroDepthBlock)) { ContextType = VisitContextType.Complex })
            };

            yield return new object[] {
                typeof(PropertyBlock),
                new BlockNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                }
            };

            yield return new object[] {
                typeof(NestedBlock),
                new BlockNode(new VisitContext(typeof(NestedBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.Complex }) {
                        new BlockNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentBlock),
                new BlockNode(new VisitContext(typeof(PropertyArgumentBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(List<string>)) { ContextType = VisitContextType.Property }) {
                        new BlockNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.PropertyGenericSegment })
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentNestedBlock),
                new BlockNode(new VisitContext(typeof(PropertyArgumentNestedBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(IDictionary<string, PropertyBlock>)) { ContextType = VisitContextType.Property }) {
                        new BlockNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.PropertyGenericSegment }),
                        new BlockNode(new VisitContext(typeof(PropertyBlock)) { ContextType = VisitContextType.Complex }) {
                            new BlockNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                        }
                    }
                }
            };

            yield return new object[] {
                typeof(TerraformValueNestedBlock),
                new BlockNode(new VisitContext(typeof(TerraformValueNestedBlock)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(ITerraformValue<TerraformValueNestedBlock.HavingString>)) { ContextType = VisitContextType.Property }) {
                        new BlockNode(new VisitContext(typeof(TerraformValueNestedBlock.HavingString)) { ContextType = VisitContextType.Complex }) {
                            new BlockNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                        }
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

        [Block]
        public class TerraformValueNestedBlock
        {
            [NestedBlock]
            public ITerraformValue<HavingString> Block { get; set; }

            [Block]
            public class HavingString
            {
                public string String { get; set; }
            }
        }
    }
}
