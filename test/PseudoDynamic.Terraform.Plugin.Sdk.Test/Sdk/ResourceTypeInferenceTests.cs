using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class ResourceTypeInferenceTests
    {
        [Fact]
        public void Resource_implementation_contains_inferred_schema()
        {
            var actualSchemaType = DesignTimeTerraformService.GetSchemaType(typeof(ResourceImpl));
            Assert.Equal(typeof(SchemaImpl), actualSchemaType);
        }

        class SchemaImpl { }

        class ResourceImpl : Resource<SchemaImpl>
        {
            public override string TypeName => "res";
        }
    }
}
