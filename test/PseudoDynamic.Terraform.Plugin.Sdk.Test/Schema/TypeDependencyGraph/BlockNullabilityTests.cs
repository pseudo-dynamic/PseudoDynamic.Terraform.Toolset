using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public class BlockNullabilityTests
    {
        [Fact]
        public void String_block_has_optional_string()
        {
            var actualBlock = BlockBuilder.Default.BuildBlock(typeof(StringBlocks.Optional));

            var expectedBlock = new BlockDefinition(typeof(StringBlocks.Optional))
            {
                Attributes = new[] {
                    new BlockAttributeDefinition(typeof(string),"nullable_string", PrimitiveDefinition.String) {
                        IsOptional = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void String_block_has_required_string()
        {
            var actualBlock = BlockBuilder.Default.BuildBlock(typeof(StringBlocks.Required));

            var expectedBlock = new BlockDefinition(typeof(StringBlocks.Required))
            {
                Attributes = new[] {
                    new BlockAttributeDefinition(typeof(string), "string", PrimitiveDefinition.String) {
                        IsRequired = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Terraform_value_block_has_optional_string()
        {
            var actualBlock = BlockBuilder.Default.BuildBlock(typeof(TeraformValueBlocks.Optional));

            var expectedBlock = new BlockDefinition(typeof(TeraformValueBlocks.Optional))
            {
                Attributes = new[] {
                    new BlockAttributeDefinition(typeof(string),"nullable_string", PrimitiveDefinition.String with {
                        OuterType = typeof(TerraformValue<string>),
                        SourceTypeWrapping = new [] { TypeWrapping.TerraformValue }
                    }) {
                        IsOptional = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Terraform_value_block_has_required_string()
        {
            var actualBlock = BlockBuilder.Default.BuildBlock(typeof(TeraformValueBlocks.Required));

            var expectedBlock = new BlockDefinition(typeof(TeraformValueBlocks.Required))
            {
                Attributes = new[] {
                    new BlockAttributeDefinition(typeof(string), "string", PrimitiveDefinition.String with {
                        OuterType = typeof(TerraformValue<string>),
                        SourceTypeWrapping = new[] { TypeWrapping.TerraformValue }
                    }) {
                        IsRequired = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        public class StringBlocks
        {
            [Block]
            public class Optional
            {
                public string? NullableString { get; set; }
            }

            [Block]
            public class Required
            {
                public string String { get; set; }
            }
        }

        public class TeraformValueBlocks
        {
            [Block]
            public class Optional
            {
                public TerraformValue<string?> NullableString { get; set; }
            }

            [Block]
            public class Required
            {
                public TerraformValue<string> String { get; set; }
            }
        }
    }
}
