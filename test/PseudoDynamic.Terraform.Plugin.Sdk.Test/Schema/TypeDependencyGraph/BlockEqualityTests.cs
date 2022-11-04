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
                                OuterType = typeof(ITerraformValue<Blocks.HavingString>),
                                SourceTypeWrapping = new [] { TypeWrapping.TerraformValue }
                            })
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.ListOfTerraformValueWrappedNestedBlocks),
                new BlockDefinition(typeof(Blocks.ListOfTerraformValueWrappedNestedBlocks)) {
                    Blocks = new []{
                        new NestedBlockAttributeDefinition(typeof(IList<ITerraformValue<Blocks.HavingString>>), "block", MonoRangeDefinition.List<IList<ITerraformValue<Blocks.HavingString>>>(new BlockDefinition(typeof(Blocks.HavingString)) {
                                Attributes = new []{
                                    new BlockAttributeDefinition(typeof(string),"string", PrimitiveDefinition.String)
                                },
                                OuterType = typeof(ITerraformValue<Blocks.HavingString>),
                                SourceTypeWrapping = new [] { TypeWrapping.TerraformValue }
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

            yield return new object[] {
                typeof(Blocks.Nullable),
                new BlockDefinition(typeof(Blocks.Nullable)) {
                    Attributes = new []{
                        new BlockAttributeDefinition(typeof(int), "nullable_number", PrimitiveDefinition.Number with {
                            SourceTypeWrapping = new [] { TypeWrapping.Nullable },
                            OuterType= typeof(int?)
                        }) {
                            IsOptional = true
                        }
                    },
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
                public ITerraformValue<HavingString> Block { get; set; }
            }

            [Block]
            internal class ListOfTerraformValueWrappedNestedBlocks
            {
                [NestedBlock]
                public IList<ITerraformValue<HavingString>> Block { get; set; }
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


            internal class InheritedStringBase
            {
                public string String { get; set; }

                internal class FloorOne : InheritedStringBase
                {
                }
            }

            [Block]
            internal class InheritedString : InheritedStringBase.FloorOne
            {
            }

            [Block]
            internal class Nullable
            {
                public int? NullableNumber { get; set; }
            }
        }
    }
}
