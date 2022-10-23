using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public class BlockEqualityTests
    {
        public static IEnumerable<object[]> GetBlockSchemas()
        {
            yield return new object[] {
                typeof(Blocks.ZeroDepth),
                new BlockDefinition(typeof(Blocks.ZeroDepth))
            };

            yield return new object[] {
                typeof(Blocks.HavingString),
                new BlockDefinition(typeof(Blocks.HavingString)) {
                    Attributes = new []{
                        new BlockAttributeDefinition(typeof(string), "string", PrimitiveDefinition.String)
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.NestedBlock),
                new BlockDefinition(typeof(Blocks.NestedBlock)) {
                    Blocks = new []{
                        new NestedBlockAttributeDefinition(typeof(Blocks.HavingString), "block", new BlockDefinition(typeof(Blocks.HavingString)) {
                                Attributes = new []{
                                    new BlockAttributeDefinition(typeof(string), "string", PrimitiveDefinition.String)
                                }
                            })
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.TerraformValueNestedBlock),
                new BlockDefinition(typeof(Blocks.TerraformValueNestedBlock)) {
                    Blocks = new []{
                        new NestedBlockAttributeDefinition(typeof(Blocks.HavingString), "block", new BlockDefinition(typeof(Blocks.HavingString)) {
                                Attributes = new []{
                                    new BlockAttributeDefinition(typeof(string),"string", PrimitiveDefinition.String)
                                },
                                WrappedSourceType = typeof(ITerraformValue<Blocks.HavingString>),
                                IsWrappedByTerraformValue = true
                            })
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.ListTerraformValueNestedBlock),
                new BlockDefinition(typeof(Blocks.ListTerraformValueNestedBlock)) {
                    Blocks = new []{
                        new NestedBlockAttributeDefinition(typeof(IList<ITerraformValue<Blocks.HavingString>>), "block", MonoRangeDefinition.List<IList<ITerraformValue<Blocks.HavingString>>>(new BlockDefinition(typeof(Blocks.HavingString)) {
                                Attributes = new []{
                                    new BlockAttributeDefinition(typeof(string),"string", PrimitiveDefinition.String)
                                },
                                WrappedSourceType = typeof(ITerraformValue<Blocks.HavingString>),
                                IsWrappedByTerraformValue = true
                            }))
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.HavingBlockList),
                new BlockDefinition(typeof(Blocks.HavingBlockList)) {
                    Blocks = new []{
                        new NestedBlockAttributeDefinition(typeof(IList<Blocks.HavingString>),"list_of_blocks", MonoRangeDefinition.List<IList<Blocks.HavingString>>(new BlockDefinition(typeof(Blocks.HavingString)) {
                                Attributes = new []{
                                    new BlockAttributeDefinition(typeof(string),"string", PrimitiveDefinition.String)
                                }
                            }))
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.ListOfStrings),
                new BlockDefinition(typeof(Blocks.ListOfStrings)) {
                    Attributes = new []{
                        new BlockAttributeDefinition(typeof(IList<string>),"list", MonoRangeDefinition.List<IList<string>>(PrimitiveDefinition.String))
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.MapOfObjects),
                new BlockDefinition(typeof(Blocks.MapOfObjects)) {
                    Attributes = new []{
                        new BlockAttributeDefinition(typeof(IDictionary<string, Blocks.HavingString>),"dictionary", new MapDefinition(typeof(IDictionary<string, Blocks.HavingString>), new ObjectDefinition(typeof(Blocks.HavingString)) {
                            Attributes = new []{
                                new ObjectAttributeDefinition(typeof(string),"string", PrimitiveDefinition.String)
                            }
                        }))
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetBlockSchemas))]
        internal void Block_schema_matches_expected_block_schema(Type schemaType, TerraformDefinition expectedDefinition)
        {
            var actualDefinition = BlockBuilder.Default.BuildBlock(schemaType);
            Assert.Equal(expectedDefinition, actualDefinition, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        public class Blocks
        {
            [Block]
            public class ZeroDepth { }

            [Block]
            public class HavingString
            {
                public string String { get; set; }
            }

            [Block]
            public class NestedBlock
            {
                [NestedBlock]
                public HavingString Block { get; set; }
            }

            [Block]
            public class TerraformValueNestedBlock
            {
                [NestedBlock]
                public ITerraformValue<HavingString> Block { get; set; }
            }

            [Block]
            public class ListTerraformValueNestedBlock
            {
                [NestedBlock]
                public IList<ITerraformValue<HavingString>> Block { get; set; }
            }

            [Block]
            public class HavingBlockList
            {
                [NestedBlock]
                public IList<HavingString> ListOfBlocks { get; set; }
            }

            [Block]
            public class ListOfStrings
            {
                public IList<string> List { get; set; }
            }

            [Block]
            public class MapOfObjects
            {
                public IDictionary<string, HavingString> Dictionary { get; set; }
            }
        }
    }
}
