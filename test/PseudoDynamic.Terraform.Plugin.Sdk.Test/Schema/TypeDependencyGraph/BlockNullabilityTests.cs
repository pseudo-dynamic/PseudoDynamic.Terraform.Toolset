namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public class BlockNullabilityTests
    {
        [Fact]
        public void String_block_has_optional_string()
        {
            var actualBlock = BlockSchemaBuilder.Default.BuildSchema(typeof(StringBlocks.Optional));

            var expectedBlock = new BlockDefinition()
            {
                Attributes = new[] {
                    new BlockAttributeDefinition("nullable_string", PrimitiveDefinition.String) {
                        IsOptional = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, TerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void String_block_has_required_string()
        {
            var actualBlock = BlockSchemaBuilder.Default.BuildSchema(typeof(StringBlocks.Required));

            var expectedBlock = new BlockDefinition()
            {
                Attributes = new[] {
                    new BlockAttributeDefinition("string", PrimitiveDefinition.String) {
                        IsRequired = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, TerraformDefinitionEqualityComparer.Default);
        }


        [Fact]
        public void Terraform_value_block_has_optional_string()
        {
            var actualBlock = BlockSchemaBuilder.Default.BuildSchema(typeof(TeraformValueBlocks.Optional));

            var expectedBlock = new BlockDefinition()
            {
                Attributes = new[] {
                    new BlockAttributeDefinition("nullable_string", PrimitiveDefinition.String with { IsWrappedByTerraformValue = true }) {
                        IsOptional = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, TerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Terraform_value_block_has_required_string()
        {
            var actualBlock = BlockSchemaBuilder.Default.BuildSchema(typeof(TeraformValueBlocks.Required));

            var expectedBlock = new BlockDefinition()
            {
                Attributes = new[] {
                    new BlockAttributeDefinition("string", PrimitiveDefinition.String with { IsWrappedByTerraformValue = true } ) {
                        IsRequired = true
                    }
                }
            };

            Assert.Equal(expectedBlock, actualBlock, TerraformDefinitionEqualityComparer.Default);
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
                public ITerraformValue<string?> NullableString { get; set; }
            }

            [Block]
            public class Required
            {
                public ITerraformValue<string> String { get; set; }
            }
        }
    }
}
