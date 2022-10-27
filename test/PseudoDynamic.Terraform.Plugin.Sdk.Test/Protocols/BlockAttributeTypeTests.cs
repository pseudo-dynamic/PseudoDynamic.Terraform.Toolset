using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Protocols
{
    public class BlockAttributeTypeTests
    {
        [Theory]
        [ClassData(typeof(BlockAttributeTypeData))]
        internal void Definition_matches_built_block_attribute_type(string expectedBlockAttributeType, TerraformDefinition definition)
        {
            var actualBlockAttributeType = BlockAttributeTypeBuilder.Default.BuildJsonType(definition);
            Assert.Equal(expectedBlockAttributeType, actualBlockAttributeType);
        }

        private class BlockAttributeTypeData : TheoryData<string, TerraformDefinition>
        {
            public BlockAttributeTypeData()
            {
                Add("\"string\"", PrimitiveDefinition.String);
                Add("\"number\"", PrimitiveDefinition.Number);
                Add("\"bool\"", PrimitiveDefinition.Bool);
                Add("\"dynamic\"", PrimitiveDefinition.Dynamic);

                Add("""["list","string"]""", MonoRangeDefinition.ListUncomputed(PrimitiveDefinition.String));
                Add("""["list",["list","string"]]""", MonoRangeDefinition.ListUncomputed(MonoRangeDefinition.ListUncomputed(PrimitiveDefinition.String)));

                Add("""["object",{"list":["list","string"]}]""", ObjectDefinition.Uncomputed() with
                {
                    Attributes = new[] {
                        ObjectAttributeDefinition.Uncomputed("list", MonoRangeDefinition.ListUncomputed(PrimitiveDefinition.String))
                    }
                });
            }
        }
    }
}
