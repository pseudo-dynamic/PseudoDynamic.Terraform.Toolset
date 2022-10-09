namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public class BlockSchemaEqualityTests
    {
        public static IEnumerable<object[]> GetBlockSchemas()
        {
            yield return new object[] {
                typeof(ZeroDepthBlock),
                new BlockDefinition()
            };

            yield return new object[] {
                typeof(PropertyBlock),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("string", PrimitiveDefinition.String) { IsRequired = true }
                    }
                }
            };

            yield return new object[] {
                typeof(NestedBlock),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("block", new BlockDefinition() {
                                Attributes = new []{
                                    new BlockAttributeDefinition("string", PrimitiveDefinition.String) { IsRequired = true }
                                }
                            }) {
                            IsRequired = true,
                        }
                    }
                }
            };

            yield return new object[] {
                typeof(TerraformValueNestedBlock),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("block", new BlockDefinition() {
                                Attributes = new []{
                                    new BlockAttributeDefinition("string", PrimitiveDefinition.String) { IsRequired = true }
                                },
                                IsWrappedByTerraformValue = true
                            }) {
                            IsRequired = true,
                        }
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentBlock),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("list", new MonoRangeDefinition(TerraformTypeConstraint.List, PrimitiveDefinition.String)) {
                            IsRequired = true,
                        }
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentNestedBlock),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("dictionary", new MapDefinition(new ObjectDefinition() {
                            Attributes = new []{
                                new ObjectAttributeDefinition("string", PrimitiveDefinition.String) { IsRequired = true }
                            }
                        })) {
                            IsRequired = true,
                        }
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetBlockSchemas))]
        internal void Block_schema_matches_expected_block_schema(Type schemaType, TerraformDefinition expectedDefinition)
        {
            var actualDefinition = BlockSchemaBuilder.Default.BuildSchema(schemaType);
            Assert.Equal(expectedDefinition, actualDefinition, TerraformDefinitionEqualityComparer.Default);
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
            [Block]
            public PropertyBlock Block { get; set; }
        }

        [Block]
        public class TerraformValueNestedBlock
        {
            [Block]
            public ITerraformValue<PropertyBlock> Block { get; set; }
        }

        [Block]
        public class PropertyArgumentBlock
        {
            public IList<string> List { get; set; }
        }

        [Block]
        public class PropertyArgumentNestedBlock
        {
            public IDictionary<string, PropertyBlock> Dictionary { get; set; }
        }
    }
}
