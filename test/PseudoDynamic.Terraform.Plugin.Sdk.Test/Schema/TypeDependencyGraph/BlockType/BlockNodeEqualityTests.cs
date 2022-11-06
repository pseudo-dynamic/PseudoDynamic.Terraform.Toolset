using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
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
                    new BlockNode(new VisitContext(typeof(IList<string>)) { ContextType = VisitContextType.Property }) {
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
                    new BlockNode(new VisitContext(typeof(TerraformValue<TerraformValueNestedBlock.HavingString>)) { ContextType = VisitContextType.Property }) {
                        new BlockNode(new VisitContext(typeof(TerraformValueNestedBlock.HavingString)) { ContextType = VisitContextType.Complex }) {
                            new BlockNode(new VisitContext(typeof(string)) { ContextType = VisitContextType.Property })
                        }
                    }
                }
            };

            yield return new object[] {
                typeof(ListOfObjects),
                  new BlockNode(new VisitContext(typeof(ListOfObjects)) { ContextType = VisitContextType.Complex }) {
                    new BlockNode(new VisitContext(typeof(IList<ListOfObjects.Object>)) { ContextType = VisitContextType.Property }) {
                        new BlockNode(new VisitContext(typeof(ListOfObjects.Object)) { ContextType = VisitContextType.Complex }) {
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
            BlockNode actualNode = new BlockNodeBuilder().BuildNode(schemaType);
            Assert.Equal(expectedNode, actualNode, BlockNodeContextEqualityComparer.Default);
        }

        [Block]
        public class ZeroDepthBlock { }

        [Block]
        public class PropertyBlock
        {
            public string String { get; set; } = null!;
        }

        [Block]
        public class NestedBlock
        {
            public PropertyBlock Schema { get; set; } = null!;
        }

        [Block]
        public class PropertyArgumentBlock
        {
            public IList<string> List { get; set; } = null!;
        }

        [Block]
        public class PropertyArgumentNestedBlock
        {
            public IDictionary<string, PropertyBlock> Dictionary { get; set; } = null!;
        }

        [Block]
        public class TerraformValueNestedBlock
        {
            [NestedBlock]
            public TerraformValue<HavingString> Block { get; set; } = null!;

            [Block]
            public class HavingString
            {
                public string String { get; set; } = null!;
            }
        }

        [Block]
        public class ListOfObjects
        {
            public IList<Object> List { get; set; } = null!;

            [Object]
            public class Object
            {
                public string String { get; set; } = null!;
            }
        }
    }
}
