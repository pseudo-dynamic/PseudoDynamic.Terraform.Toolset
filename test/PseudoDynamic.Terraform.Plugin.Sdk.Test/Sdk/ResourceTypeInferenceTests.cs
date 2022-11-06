using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class ResourceTypeInferenceTests
    {
        [Fact]
        public void Resource_implementation_contains_inferred_schema()
        {
            Type actualSchemaType = DesignTimeTerraformService.GetSchemaType(typeof(ResourceImpl));
            Assert.Equal(typeof(SchemaImpl), actualSchemaType);
        }

        private class SchemaImpl { }

        private class ResourceImpl : Resource<SchemaImpl>
        {
            public override string TypeName => "res";
        }
    }
}
