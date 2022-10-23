using FluentAssertions;
using Moq;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using PseudoDynamic.Terraform.Plugin.Infrastructure.Fakes;
using PseudoDynamic.Terraform.Plugin.Schema;
using System.Numerics;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class TerraformValidateTests : IClassFixture<PluginHostFixtures.ProtocolV5>
    {
        private readonly PluginHostFixtureBase _pluginHostFixture;

        public TerraformValidateTests(PluginHostFixtures.ProtocolV5 pluginHostFixture) =>
            _pluginHostFixture = pluginHostFixture;

        public static IEnumerable<object?[]> Produce_terraform_validatable_config_schemas()
        {
            // unknown
            foreach (var resource in UnknownResources(default(string), "config_string_unknown", true)) yield return resource;

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
            foreach (var resource in NestedBlockResources("nested", "config_nested_block")) yield return resource;
            foreach (var resource in NestedBlockRangeResources(SchemaFake<string>.Block.RangeList("first_nested", "second_nested"), "config_nested_block_list")) yield return resource;
            foreach (var resource in NestedBlockRangeResources(SchemaFake<string>.Block.RangeSet("tf_second_csharp_first", "tf_first_csharp_second"), "config_nested_block_set")) yield return resource;

            IEnumerable<object?[]> AdvancedResources<T>(
                T value,
                string fileName,
                bool isUnknown = false,
                IEqualityComparer<T>? equalityComparer = null,
                bool isNestedBlock = false,
                bool withoutWrapping = false)
            {
                var filePattern = $"{fileName}.tf";
                yield return new object[] { new SchemaFake<T>(value, equalityComparer) { IsNestedBlock = isNestedBlock, }.Schema, filePattern };
                if (!withoutWrapping) yield return new object[] { new SchemaFake<T>.TerraformValueFake(TerraformValue.OfValue<T>(value, isUnknown), equalityComparer) { IsNestedBlock = isNestedBlock }.Schema, filePattern };
            }

            IEnumerable<object?[]> UnknownResources<T>(T value, string fileName, bool isUnknown, IEqualityComparer<T>? equalityComparer = null) =>
                AdvancedResources<T>(value, fileName, isUnknown: isUnknown, equalityComparer: equalityComparer);

            IEnumerable<object?[]> Resources<T>(T value, string fileName) =>
                AdvancedResources<T>(value, fileName, isUnknown: false, equalityComparer: null);

            IEnumerable<object?[]> NestedBlockResources<T>(T value, string fileName) =>
                AdvancedResources<SchemaFake<T>.Block>(new SchemaFake<T>.Block(value), fileName, isNestedBlock: true);

            IEnumerable<object?[]> NestedBlockRangeResources<T>(T value, string fileName) =>
                AdvancedResources<T>(value, fileName, isNestedBlock: true, withoutWrapping: true);

            IList<T> List<T>(params T[] items) => new List<T>(items);
            ISet<T> Set<T>(params T[] items) => new HashSet<T>(items);
            IDictionary<string, T> Map<T>(params (string, T)[] items) => new Dictionary<string, T>(items.Select(x => new KeyValuePair<string, T>(x.Item1, x.Item2)));
        }

        [Theory]
        [MemberData(nameof(Produce_terraform_validatable_config_schemas))]
        internal async Task Terraform_validate_config_schema<Resource, Schema, Value>(Schema expectedConfig, string filePattern)
            where Schema : class, ISchemaFake
        {
            var resourceMock = new Mock<IResource<Schema>>();
            resourceMock.SetupGet(x => x.TypeName).Returns("validate").Verifiable();

            ValidateConfig.Context<Schema>? actualContext = null;
            resourceMock.Setup(x => x.ValidateConfig(It.IsAny<ValidateConfig.Context<Schema>>())).Callback((ValidateConfig.Context<Schema> context) => actualContext = context);

            _pluginHostFixture.Provider.ReplaceResourceDefinition(new ResourceDescriptor(resourceMock.Object, typeof(Schema)));

            using var terraform = _pluginHostFixture.CreateTerraformCommand("TerraformProjects/resource-validate", filePattern);
            Record.Exception(terraform.Validate).Should().BeNull();

            resourceMock.Verify();
            Assert.Equal(expectedConfig, actualContext?.Config);
        }
    }
}
