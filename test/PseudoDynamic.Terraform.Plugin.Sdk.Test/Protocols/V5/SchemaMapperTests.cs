using AutoMapper;
using Google.Protobuf;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Protocols.V5
{
    public class SchemaMapperTests
    {
        private static IMapper Mapper = new MapperConfiguration(config => config.AddProfile<SchemaMapper>()).CreateMapper();

        [Fact]
        internal void Mapper_configuration_is_valid() =>
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();

        [Theory]
        [ClassData(typeof(SchemaBlockData))]
        internal void Block_definition_maps_to_schema_block(Schema.Types.Block expectedSchemaBlock, BlockDefinition blockDefinition)
        {
            var actualSchemaBlock = Mapper.Map<BlockDefinition, Schema.Types.Block>(blockDefinition);
            Assert.Equal(expectedSchemaBlock, actualSchemaBlock);
        }

        private class SchemaBlockData : TheoryData<Schema.Types.Block, BlockDefinition>
        {
            public SchemaBlockData()
            {
                Add(new Schema.Types.Block()
                {
                    Description = "goofy",
                    DescriptionKind = StringKind.Markdown,
                    Deprecated = true,
                    Version = 2
                }, new BlockDefinition()
                {
                    Description = "goofy",
                    DescriptionKind = DescriptionKind.Markdown,
                    IsDeprecated = true,
                    Version = 2
                });

                var stringList = MonoRangeDefinition.List(PrimitiveDefinition.String);

                Add(new Schema.Types.Block()
                {
                    Version = 1,
                    Attributes = {
                        {
                            new Schema.Types.Attribute() {
                                Name = "list",
                                Required = true,
                                Type = ByteString.CopyFromUtf8(BlockAttributeTypeBuilder.Default.BuildJsonType(stringList))
                            }
                        }
                    }
                }, new BlockDefinition()
                {
                    Attributes = new[] {
                        new BlockAttributeDefinition("list", stringList)
                    }
                });

                Add(new Schema.Types.Block()
                {
                    Version = 1,
                    BlockTypes = {
                        {
                            new Schema.Types.NestedBlock() {
                                TypeName = "list",
                                Nesting = Schema.Types.NestedBlock.Types.NestingMode.List,
                                Block = new Schema.Types.Block() {
                                    Version = 1
                                }
                            }
                        }
                    }
                }, new BlockDefinition()
                {
                    Blocks = new[] {
                        new NestedBlockAttributeDefinition("list", MonoRangeDefinition.List(new BlockDefinition()))
                    }
                });
            }
        }
    }
}
