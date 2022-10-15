namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public class BlockVersionTests
    {
        [Fact]
        public void Block_schema_should_have_default_schema_Version()
        {
            var actualBlock = BlockBuilder.Default.BuildBlock(typeof(Blocks.Default));
            var expectedBlock = new BlockDefinition() { Version = BlockDefinition.DefaultVersion };
            Assert.Equal(expectedBlock, actualBlock, TerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Block_schema_should_have_custom_schema_Version()
        {
            var actualBlock = BlockBuilder.Default.BuildBlock(typeof(Blocks.SchemaVersion));
            var expectedBlock = new BlockDefinition() { Version = 2 };
            Assert.Equal(expectedBlock, actualBlock, TerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Nested_block_schema_should_have_custom_schema_Version()
        {
            var actualBlock = BlockBuilder.Default.BuildBlock(typeof(Blocks.NestedSchemaVersion));

            var expectedBlock = new BlockDefinition()
            {
                Blocks = new[] {
                    new NestedBlockAttributeDefinition("block", new BlockDefinition() { Version = 2 })
                }
            };

            Assert.Equal(expectedBlock, actualBlock, TerraformDefinitionEqualityComparer.Default);
        }

        public class Blocks
        {
            [Block]
            public class Default
            {
            }

            [Block(2)]
            public class SchemaVersion
            {
            }

            [Block]
            public class NestedSchemaVersion
            {
                [NestedBlock]
                public SchemaVersion Block { get; set; }
            }
        }
    }
}
