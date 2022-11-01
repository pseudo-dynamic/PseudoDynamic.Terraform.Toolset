using Moq;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using PseudoDynamic.Terraform.Plugin.Infrastructure.Fakes;
using PseudoDynamic.Terraform.Plugin.Sdk.Services;
using System.Numerics;
using static PseudoDynamic.Terraform.Plugin.Infrastructure.CollectionFactories;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class TerraformValidateIntegrationTests : IClassFixture<PluginHostFixtures.ProtocolV5>
    {
        private readonly PluginHostFixtureBase _pluginHostFixture;

        public TerraformValidateIntegrationTests(PluginHostFixtures.ProtocolV5 pluginHostFixture) =>
            _pluginHostFixture = pluginHostFixture;

        public static IEnumerable<object?[]> Produce_terraform_validatable_config_schemas()
        {
            // unknown
            foreach (var resource in Resources(default(string), "config_string_unknown", isUnknown: true)) yield return resource;
            foreach (var resource in Resources<ITerraformValue>(TerraformValue.OfUnknown<object>(), "config_string_unknown", isUnknown: true, notWrappable: true)) yield return resource;

            // string
            foreach (var resource in Resources("Hello from Terraform!", "config_string")) yield return resource;

            // number
            foreach (var resource in Resources(int.MaxValue, "config_int32_max")) yield return resource;
            foreach (var resource in Resources(long.MaxValue, "config_int64_max")) yield return resource;
            foreach (var resource in Resources(new BigInteger(Enumerable.Range(0, 16).Select(x => byte.MaxValue).ToArray(), isUnsigned: true), "config_int128_max")) yield return resource;
            foreach (var resource in Resources(float.MaxValue, "config_float_max")) yield return resource;
            foreach (var resource in Resources(double.MaxValue, "config_double_max")) yield return resource;

            // bool
            foreach (var resource in Resources(true, "config_bool")) yield return resource;

            // list
            foreach (var resource in Resources(List("first", "second"), "config_list")) yield return resource;

            // set
            foreach (var resource in Resources(Set("tf_second_csharp_first", "tf_first_csharp_second"), "config_set")) yield return resource;

            // map
            foreach (var resource in Resources(Map(("zero", "example")), "config_map")) yield return resource;

            // object
            foreach (var resource in Resources(new SchemaFake<string>.Object("disney world"), "config_object")) yield return resource;
            foreach (var resource in Resources(SchemaFake<string>.Object.HavingList("disney", "world"), "config_object_having_list")) yield return resource;
            foreach (var resource in Resources(SchemaFake<string>.Object.RangeList("first", "second"), "config_object_list")) yield return resource;

            // nested block
            foreach (var resource in Resources(new SchemaFake<string>.Block("nested"), "config_nested_block", isNestedBlock: true)) yield return resource;

            foreach (var resource in Resources(
                SchemaFake<string>.Block.RangeList("first_nested", "second_nested"),
                "config_nested_block_list",
                isNestedBlock: true,
                notWrappable: true))
                yield return resource;

            foreach (var resource in Resources(
                SchemaFake<string>.Block.RangeSet("tf_second_csharp_first", "tf_first_csharp_second"),
                "config_nested_block_set",
                isNestedBlock: true,
                notWrappable: true))
                yield return resource;

            foreach (var resource in Resources(SchemaFake<string>.Block.RangeMap(
                    ("first_nested_block", "first_nested_block_attribute"),
                    ("second_nested_block", "second_nested_block_attribute")),
                "config_nested_block_map",
                isNestedBlock: true,
                notWrappable: true))
                yield return resource;

            IEnumerable<object?[]> Resources<T>(
                T value,
                string fileName,
                bool isUnknown = false,
                IEqualityComparer<T>? equalityComparer = null,
                bool isNestedBlock = false,
                bool notWrappable = false)
            {
                var filePattern = $"{fileName}.tf";
                yield return new object[] { new SchemaFake<T>(value, equalityComparer) { IsNestedBlock = isNestedBlock, }.Schema, filePattern };
                if (!notWrappable) yield return new object[] { new SchemaFake<T>.TerraformValueFake(TerraformValue.OfValue<T>(value, isUnknown), equalityComparer) { IsNestedBlock = isNestedBlock }.Schema, filePattern };
            }
        }

        [Theory]
        [MemberData(nameof(Produce_terraform_validatable_config_schemas))]
        internal async Task Terraform_validate_config_schema<Schema>(Schema expectedConfig, string filePattern)
            where Schema : class, ISchemaFake
        {
            var resourceMock = new Mock<IResource<Schema>>();
            resourceMock.SetupGet(x => x.Name).Returns("validate").Verifiable();

            Resource.ValidateContext<Schema>? actualContext = null;
            resourceMock.Setup(x => x.ValidateConfig(It.IsAny<Resource.ValidateContext<Schema>>())).Callback((Resource.ValidateContext<Schema> context) => actualContext = context);

            _pluginHostFixture.Provider.ReplaceResource(new ResourceServiceDescriptor(typeof(IResource<Schema>), typeof(Schema), typeof(object)) { Implementation = resourceMock.Object });

            using var terraform = _pluginHostFixture.CreateTerraformCommand("TerraformProjects/resource-validate", filePattern);
            await terraform.Init();
            await terraform.Validate();

            resourceMock.Verify();
            Assert.Equal(expectedConfig, actualContext?.Config);
        }
    }
}
