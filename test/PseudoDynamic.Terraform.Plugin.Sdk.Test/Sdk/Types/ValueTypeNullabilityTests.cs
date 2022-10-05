using PseudoDynamic.Terraform.Plugin.Internals;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Types
{
    public class ValueTypeNullabilityTests
    {
        [Theory]
        [InlineData(new object[] { "integer" })]
        [InlineData(new object[] { "string" })]
        [InlineData(new object[] { "bool" })]
        [InlineData(new object[] { "any" })]
        [InlineData(new object[] { "list_of_string" })]
        [InlineData(new object[] { "set_of_string" })]
        [InlineData(new object[] { "map_of_string" })]
        [InlineData(new object[] { "object" })]
        [InlineData(new object[] { "list_of_object" })]
        [InlineData(new object[] { "set_of_object" })]
        [InlineData(new object[] { "map_of_object" })]
        [InlineData(new object[] { "tuple" })]
        [InlineData(new object[] { "single_nested_block" })]
        [InlineData(new object[] { "list_of_nested_block" })]
        [InlineData(new object[] { "set_of_nested_block" })]
        [InlineData(new object[] { "map_of_nested_block" })]
        public void Required_attribute_is_required(string attributeName)
        {
            Assert.True(NullableBlock.DefaultSchema.Attributes[attributeName].IsRequired, $"Attribute \"{attributeName}\" should be marked as required");
            Assert.False(NullableBlock.DefaultSchema.Attributes[attributeName].IsOptional, $"Attribute \"{attributeName}\" should not be marked as optional");
        }

        [Theory]
        [InlineData(new object[] { "nullable_integer" })]
        [InlineData(new object[] { "nullable_string" })]
        [InlineData(new object[] { "nullable_bool" })]
        [InlineData(new object[] { "nullable_any" })]
        [InlineData(new object[] { "nullable_list_of_string" })]
        [InlineData(new object[] { "nullable_set_of_string" })]
        [InlineData(new object[] { "nullable_map_of_string" })]
        [InlineData(new object[] { "nullable_object" })]
        [InlineData(new object[] { "nullable_tuple" })]
        [InlineData(new object[] { "nullable_single_nested_block" })]
        public void Optional_attribute_is_optional(string attributeName)
        {
            Assert.True(NullableBlock.DefaultSchema.Attributes[attributeName].IsOptional, $"Attribute \"{attributeName}\" should be marked as optional");
            Assert.False(NullableBlock.DefaultSchema.Attributes[attributeName].IsRequired, $"Attribute \"{attributeName}\" should not be marked as required");
        }
    }
}
