using CSF.Collections;
using FluentAssertions;
using Moq;
using PseudoDynamic.Terraform.Plugin.Infrastructure;
using PseudoDynamic.Terraform.Plugin.Schema;
using System.Numerics;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class TerraformValidateTests : IClassFixture<PluginHostFixtures.ProtocolV5>
    {
        private readonly PluginHostFixtureBase _pluginHostFixture;

        public TerraformValidateTests(PluginHostFixtures.ProtocolV5 pluginHostFixture) =>
            _pluginHostFixture = pluginHostFixture;

        public static IEnumerable<object?[]> Produce_terraform_validate_config_values()
        {
            // unknown
            foreach (var resource in CreateUnknownResources(default(string), "config_string_unknown", true)) yield return resource;

            // string
            foreach (var resource in CreateResources("Hello from Terraform!", "config_string")) yield return resource;

            // number
            foreach (var resource in CreateResources(int.MaxValue, "config_int32_max")) yield return resource;
            foreach (var resource in CreateResources(long.MaxValue, "config_int64_max")) yield return resource;
            foreach (var resource in CreateResources(new BigInteger(Enumerable.Range(0, 16).Select(x => byte.MaxValue).ToArray(), isUnsigned: true), "config_int128_max")) yield return resource;
            foreach (var resource in CreateResources(float.MaxValue, "config_float_max")) yield return resource;
            foreach (var resource in CreateResources(double.MaxValue, "config_double_max")) yield return resource;

            // bool
            foreach (var resource in CreateResources(true, "config_bool")) yield return resource;

            // list
            foreach (var resource in CreateComparableResources<IList<string>>(List("first", "second"), "config_list", new ListEqualityComparer<string>())) yield return resource;

            // set
            foreach (var resource in CreateComparableResources<ISet<string>>(Set("tf_second_csharp_first", "tf_first_csharp_second"), "config_set", new SetEqualityComparer<string>())) yield return resource;

            // map
            foreach (var resource in CreateComparableResources<IDictionary<string, string>>(
                Map(("zero", "example")), "config_map",
                new ListEqualityComparer<KeyValuePair<string, string>>()))
                yield return resource;

            // object
            foreach (var resource in CreateObjectResources(new SchemaFake<string>.ValueHavingSchema("disney world"), "config_object")) yield return resource;

            foreach (var resource in CreateObjectResources(new SchemaFake<IList<string>>.ValueHavingSchema(List("disney", "world"), new ListEqualityComparer<string>()), "config_object_nested_list"))
                yield return resource;

            foreach (var resource in CreateObjectResources<IList<SchemaFake<string>.ObjectBeingSchema>>(
                ListFactory(SchemaFake<string>.ObjectBeingSchema.OfValue, "first", "second"),
                "config_object_list",
                new ListEqualityComparer<SchemaFake<string>.ObjectBeingSchema>()))
                yield return resource;

            // nested block
            foreach (var resource in CreateNestedBlockResources("nested", "config_nested_block")) yield return resource;
            foreach (var resource in CreateNestedBlockResources("nested", "config_nested_block")) yield return resource;
            foreach (var resource in CreateNestedBlockListResources(List("first_nested", "second_nested"), "config_nested_block_list")) yield return resource;
            foreach (var resource in CreateNestedBlockSetResources(Set("tf_second_csharp_first", "tf_first_csharp_second"), "config_nested_block_set")) yield return resource;

            IEnumerable<object?[]> CreateFeaturedResources<T>(
                T value,
                string fileName,
                bool isUnknown = false,
                IEqualityComparer<T>? equalityComparer = null,
                bool isNestedBlock = false,
                bool withoutWrapping = false)
            {
                var filePattern = $"{fileName}.tf";
                yield return new object[] { new SchemaFake<T>(value, equalityComparer) { IsNestedBlock = isNestedBlock, }.Schema, filePattern };
                if (!withoutWrapping) yield return new object[] { new SchemaFake<T>.TerraformValue(TerraformValue.OfValue<T>(value, isUnknown), equalityComparer) { IsNestedBlock = isNestedBlock }.Schema, filePattern };
            }

            IEnumerable<object?[]> CreateUnknownResources<T>(T value, string fileName, bool isUnknown, IEqualityComparer<T>? equalityComparer = null) =>
                CreateFeaturedResources<T>(value, fileName, isUnknown: isUnknown, equalityComparer: equalityComparer);

            IEnumerable<object?[]> CreateResources<T>(T value, string fileName) =>
                CreateFeaturedResources<T>(value, fileName, isUnknown: false, equalityComparer: null);

            IEnumerable<object?[]> CreateComparableResources<T>(T value, string fileName, IEqualityComparer<T>? equalityComparer) =>
                CreateFeaturedResources<T>(value, fileName, isUnknown: false, equalityComparer);

            IEnumerable<object?[]> CreateObjectResources<T>(T value, string fileName, IEqualityComparer<T>? equalityComparer = null) =>
                CreateFeaturedResources<T>(value, fileName, equalityComparer: equalityComparer);

            IEnumerable<object?[]> CreateNestedBlockResources<T>(T value, string fileName) =>
                CreateFeaturedResources<SchemaFake<T>.ValueHavingSchema>(new SchemaFake<T>.ValueHavingSchema(value), fileName, isNestedBlock: true);

            IEnumerable<object?[]> CreateNestedBlockListResources<T>(IList<T> values, string fileName) =>
                CreateFeaturedResources<IList<SchemaFake<T>.ValueHavingSchema>>(
                    ListFactory(SchemaFake<T>.ValueHavingSchema.OfValue, values.ToArray()),
                    fileName,
                    equalityComparer: new ListEqualityComparer<SchemaFake<T>.ValueHavingSchema>(),
                    isNestedBlock: true,
                    withoutWrapping: true);

            IEnumerable<object?[]> CreateNestedBlockSetResources<T>(ISet<T> values, string fileName) =>
                CreateFeaturedResources<ISet<SchemaFake<T>.ValueHavingSchema>>(
                    values.Select(x => new SchemaFake<T>.ValueHavingSchema(x)).ToHashSet(),
                    fileName,
                    equalityComparer: new SetEqualityComparer<SchemaFake<T>.ValueHavingSchema>(),
                    isNestedBlock: true,
                    withoutWrapping: true);

            IList<T> List<T>(params T[] items) => new List<T>(items);
            IList<TList> ListFactory<TList, TValue>(Func<TValue, TList> createList, params TValue[] values) => values.Select(createList).ToList();
            ISet<T> Set<T>(params T[] items) => new HashSet<T>(items);
            IDictionary<string, T> Map<T>(params (string, T)[] items) => new Dictionary<string, T>(items.Select(x => new KeyValuePair<string, T>(x.Item1, x.Item2)));
        }

        [Theory]
        [MemberData(nameof(Produce_terraform_validate_config_values))]
        internal async void Terraform_validate_config_value<Resource, Schema, Value>(Schema expectedConfig, string filePattern)
            where Schema : class, ISchemaFake
        {
            var resourceMock = new Mock<IResource<Schema>>();
            resourceMock.SetupGet(x => x.TypeName).Returns("validate").Verifiable();

            ValidateConfig.Context<Schema>? actualContext = null;
            resourceMock.Setup(x => x.ValidateConfig(It.IsAny<ValidateConfig.Context<Schema>>())).Callback((ValidateConfig.Context<Schema> context) => actualContext = context);

            _pluginHostFixture.Provider.ReplaceResourceDefinition(new ResourceDescriptor(resourceMock.Object, typeof(Schema)));

            using var terraform = _pluginHostFixture.CreateTerraformCommand("TerraformProjects/resource_validate", filePattern);
            Record.Exception(terraform.Validate).Should().BeNull();

            resourceMock.Verify();
            Assert.Equal(expectedConfig, actualContext?.Config);
        }
    }
}
