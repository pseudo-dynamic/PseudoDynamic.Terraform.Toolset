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
                Add("\"dynamic\"", PrimitiveDefinition.Any);

                Add("""["list","string"]""", MonoRangeDefinition.List(PrimitiveDefinition.String));
                Add("""["list",["list","string"]]""", MonoRangeDefinition.List(MonoRangeDefinition.List(PrimitiveDefinition.String)));

                Add("""["object",{"list":["list","string"]}]""", new ObjectDefinition()
                {
                    Attributes = new[] {
                        new ObjectAttributeDefinition("list", MonoRangeDefinition.List(PrimitiveDefinition.String))
                    }
                });
            }
        }
    }
}
