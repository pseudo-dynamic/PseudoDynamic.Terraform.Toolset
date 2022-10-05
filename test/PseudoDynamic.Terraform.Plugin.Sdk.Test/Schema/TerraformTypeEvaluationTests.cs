using PseudoDynamic.Terraform.Plugin.Internals;
using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    public class TerraformTypeEvaluationTests
    {
        [Theory]
        [InlineData(new object[] { "signed_byte", TerraformType.Number })]
        [InlineData(new object[] { "byte", TerraformType.Number })]
        [InlineData(new object[] { "unsigned_short", TerraformType.Number })]
        [InlineData(new object[] { "short", TerraformType.Number })]
        [InlineData(new object[] { "unsigned_integer", TerraformType.Number })]
        [InlineData(new object[] { "integer", TerraformType.Number })]
        [InlineData(new object[] { "unsigned_long", TerraformType.Number })]
        [InlineData(new object[] { "long", TerraformType.Number })]
        [InlineData(new object[] { "big_integer", TerraformType.Number })]
        [InlineData(new object[] { "string", TerraformType.String })]
        [InlineData(new object[] { "bool", TerraformType.Bool })]
        [InlineData(new object[] { "any", TerraformType.Any })]
        [InlineData(new object[] { "list_of_string", TerraformType.List })]
        [InlineData(new object[] { "set_of_string", TerraformType.Set })]
        [InlineData(new object[] { "map_of_string", TerraformType.Map })]
        [InlineData(new object[] { "object", TerraformType.Object })]
        [InlineData(new object[] { "nested_block", TerraformType.NestedBlock })]
        public void Attribute_type_should_match_terraform_type(string attributeName, TerraformType expectedTerraformType)
        {
            Assert.Equal(expectedTerraformType, NullableBlock.DefaultSchema.Attributes[attributeName].TerraformType);
        }
    }
}
