using PseudoDynamic.Terraform.Plugin.Infrastructure.Fakes;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using System.Numerics;
using static PseudoDynamic.Terraform.Plugin.Infrastructure.CollectionFactories;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    public class TerraformDynamicMessagePackEncoderTests
    {
        private readonly static DynamicDefinitionResolver DynamicResolver = new();
        private readonly static TerraformDynamicMessagePackDecoder Decoder = new(new NonRequestableServiceProvider(), DynamicResolver);
        private readonly static TerraformDynamicMessagePackEncoder Encoder = new(new());

        [Theory]
        [ClassData(typeof(DynamicSchemasGenerator))]
        internal void Encoder_should_encode_dynamic(object content, Type contentType, Type dynamicType)
        {
            var encoder = new TerraformDynamicMessagePackEncoder(new());

            var dynamicBlock = BlockBuilder.Default.BuildBlock(dynamicType);
            var encoded = encoder.Encode(dynamicBlock, content);

            var block = BlockBuilder.Default.BuildBlock(contentType);
            var decoded = Decoder.Decode(encoded, block, new());

            Assert.Equal(content, decoded);
        }

        [Theory]
        [ClassData(typeof(SchemasGenerator))]
        internal void Encoder_should_encode_then_decode_schemas(object content, Type contentType)
        {
            var block = BlockBuilder.Default.BuildBlock(contentType);
            var bytes = Encoder.Encode(block, content);
            var result = Decoder.Decode(bytes, block, new());
            Assert.Equal(content, result);
        }

        internal class SchemasGeneratorBase : TheoryData
        {
            protected Type CurrentDynamicType { get; private set; } = null!;

            public SchemasGeneratorBase()
            {
                // number
                Add(byte.MaxValue);
                Add(sbyte.MaxValue);
                Add(ushort.MaxValue);
                Add(short.MaxValue);
                Add(uint.MaxValue);
                Add(int.MaxValue);
                Add(float.MaxValue);
                Add(ulong.MaxValue);
                Add(long.MaxValue);
                Add(double.MaxValue);
                Add(new BigInteger(Enumerable.Range(0, 16).Select(x => byte.MaxValue).ToArray(), isUnsigned: true)); // 128 bit

                // string
                Add("works");

                // bool
                Add(true);

                // list
                Add(List("first", "second"));

                // set
                Add(Set("first", "second"));

                // map
                Add(Map(("key", "value")));

                // object
                Add(new SchemaFake<string>.Object("disney world"));
                Add(SchemaFake<string>.Object.HavingList("disney", "world"));
                Add(SchemaFake<string>.Object.RangeList("first", "second"));

                // nested block
                Add(new SchemaFake<string>.Block("nested"), isNested: true);

                Add(SchemaFake<string>.Block.RangeList("first", "second"), isNested: true, notWrappable: true);
                Add(SchemaFake<string>.Block.RangeSet("first", "second"), isNested: true, notWrappable: true);

                Add(SchemaFake<string>.Block.RangeMap(
                        ("first_nested_block", "first_nested_block_attribute"),
                        ("second_nested_block", "second_nested_block_attribute")),
                    isNested: true,
                    notWrappable: true);
            }

            protected virtual void Add(params object[] row) => AddRow(row);

            private void Add<T>(T value, bool isNested = false, bool notWrappable = false)
                where T : notnull
            {
                {
                    var schema = new SchemaFake<T>(value) { IsNestedBlock = isNested }.Schema;
                    CurrentDynamicType = new SchemaFake<object>(value) { IsNestedBlock = isNested }.Schema.GetType();
                    Add(schema, schema.GetType());
                }

                if (!notWrappable)
                {
                    var schema = new SchemaFake<T>.TerraformValueFake(TerraformValue.OfValue(value)) { IsNestedBlock = isNested }.Schema;
                    CurrentDynamicType = new SchemaFake<object>.TerraformValueFake(TerraformValue.OfValue((object)value)) { IsNestedBlock = isNested }.Schema.GetType();
                    Add(schema, schema.GetType());
                }
            }
        }

        internal class SchemasGenerator : SchemasGeneratorBase
        {
        }

        internal class DynamicSchemasGenerator : SchemasGeneratorBase
        {
            protected override void Add(params object[] row) =>
                AddRow(row.Concat(new[] { CurrentDynamicType }).ToArray());
        }

        private class NonRequestableServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType) => throw new NotImplementedException();
        }
    }
}
