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
                                OuterType = typeof(TerraformValue<Blocks.HavingString>),
                                SourceTypeWrapping = TypeWrapping.TerraformValue
                            })
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.ListOfTerraformValueWrappedNestedBlocks),
                new BlockDefinition(typeof(Blocks.ListOfTerraformValueWrappedNestedBlocks)) {
                    Blocks = new []{
                        new NestedBlockAttributeDefinition(typeof(IList<TerraformValue<Blocks.HavingString>>), "block", MonoRangeDefinition.List<IList<TerraformValue<Blocks.HavingString>>>(new BlockDefinition(typeof(Blocks.HavingString)) {
                                Attributes = new []{
                                    new BlockAttributeDefinition(typeof(string),"string", PrimitiveDefinition.String)
                                },
                                OuterType = typeof(TerraformValue<Blocks.HavingString>),
                                SourceTypeWrapping = TypeWrapping.TerraformValue
                            }))
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.ListOfBlocks),
                new BlockDefinition(typeof(Blocks.ListOfBlocks)) {
                    Blocks = new []{
                        new NestedBlockAttributeDefinition(typeof(IList<Blocks.HavingString>),"list", MonoRangeDefinition.List<IList<Blocks.HavingString>>(new BlockDefinition(typeof(Blocks.HavingString)) {
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
                typeof(Blocks.ListOfObjects),
                new BlockDefinition(typeof(Blocks.ListOfObjects)) {
                    Attributes = new []{
                        new BlockAttributeDefinition(typeof(IList<Blocks.HavingString>),"list", MonoRangeDefinition.List<IList<Blocks.HavingString>>(new ObjectDefinition(typeof(Blocks.HavingString)) {
                                Attributes = new []{
                                    new ObjectAttributeDefinition(typeof(string),"string", PrimitiveDefinition.String)
                                }
                            }))
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

            yield return new object[] {
                typeof(Blocks.InheritedString),
                new BlockDefinition(typeof(Blocks.InheritedString)) {
                    Attributes = new []{
                        new BlockAttributeDefinition(typeof(string), "string", PrimitiveDefinition.String)
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

        class Blocks
        {
            [Block]
            internal class ZeroDepth { }

            [Block]
            internal class HavingString
            {
                public string String { get; set; }
            }

            [Block]
            internal class NestedBlock
            {
                [NestedBlock]
                public HavingString Block { get; set; }
            }

            [Block]
            internal class TerraformValueNestedBlock
            {
                [NestedBlock]
                public TerraformValue<HavingString> Block { get; set; }
            }

            [Block]
            internal class ListOfTerraformValueWrappedNestedBlocks
            {
                [NestedBlock]
                public IList<TerraformValue<HavingString>> Block { get; set; }
            }

            [Block]
            internal class ListOfBlocks
            {
                [NestedBlock]
                public IList<HavingString> List { get; set; }
            }

            [Block]
            internal class ListOfStrings
            {
                public IList<string> List { get; set; }
            }

            [Block]
            internal class ListOfObjects
            {
                public IList<HavingString> List { get; set; }
            }

            [Block]
            internal class MapOfObjects
            {
                public IDictionary<string, HavingString> Dictionary { get; set; }
            }


            internal class InheritableStringFloorZero
            {
                public string String { get; set; }

                internal class FloorOne : InheritableStringFloorZero
                {
                }
            }

            [Block]
            internal class InheritedString : InheritableStringFloorZero.FloorOne
            {

            }
        }
    }
}
