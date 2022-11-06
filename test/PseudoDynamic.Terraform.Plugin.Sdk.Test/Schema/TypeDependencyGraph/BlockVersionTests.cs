using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public class BlockVersionTests
    {
        [Fact]
        public void Block_schema_should_have_default_schema_Version()
        {
            BlockDefinition actualBlock = BlockBuilder.Default.BuildBlock(typeof(Blocks.Default));
            BlockDefinition expectedBlock = new(typeof(Blocks.Default)) { Version = BlockDefinition.DefaultVersion };
            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Block_schema_should_have_custom_schema_Version()
        {
            BlockDefinition actualBlock = BlockBuilder.Default.BuildBlock(typeof(Blocks.SchemaVersion));
            BlockDefinition expectedBlock = new(typeof(Blocks.SchemaVersion)) { Version = 2 };
            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Nested_block_schema_should_have_schema_version()
        {
            BlockDefinition actualBlock = BlockBuilder.Default.BuildBlock(typeof(Blocks.NestedSchemaVersion));

            BlockDefinition expectedBlock = new(typeof(Blocks.NestedSchemaVersion)) {
                Blocks = new[] {
                    new NestedBlockAttributeDefinition(typeof(Blocks.SchemaVersion),"block", new BlockDefinition(typeof(Blocks.SchemaVersion)) { Version = 2 })
                }
            };

            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
        }

        [Fact]
        public void Nested_block_schema_should_have_overriden_schema_Version()
        {
            BlockDefinition actualBlock = BlockBuilder.Default.BuildBlock(typeof(Blocks.NestedSchemaOverridenVersion));

            BlockDefinition expectedBlock = new(typeof(Blocks.NestedSchemaOverridenVersion)) {
                Blocks = new[] {
                    new NestedBlockAttributeDefinition(typeof(Blocks.SchemaVersion), "block", new BlockDefinition(typeof(Blocks.SchemaVersion)) { Version = 3 })
                }
            };

            Assert.Equal(expectedBlock, actualBlock, AssertingTerraformDefinitionEqualityComparer.Default);
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
                public SchemaVersion Block { get; set; } = null!;
            }

            [Block]
            public class NestedSchemaOverridenVersion
            {
                [NestedBlock(3)]
                public SchemaVersion Block { get; set; } = null!;
            }
        }
    }
}
