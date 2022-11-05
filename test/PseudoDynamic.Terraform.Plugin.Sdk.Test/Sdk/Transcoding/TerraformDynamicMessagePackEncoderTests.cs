using System.Numerics;
using PseudoDynamic.Terraform.Plugin.Infrastructure.Fakes;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using static PseudoDynamic.Terraform.Plugin.Infrastructure.CollectionFactories;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    public class TerraformDynamicMessagePackEncoderTests
    {
        private readonly static BlockBuilder BlockBuilder = BlockBuilder.Default;
        private readonly static SchemaBuilder SchemaBuilder = new();
        private readonly static TerraformDynamicMessagePackDecoder Decoder = new(new NonRequestableServiceProvider(), SchemaBuilder);
        private readonly static TerraformDynamicMessagePackEncoder Encoder = new(new());

        [Theory]
        [ClassData(typeof(DynamicSchemasGenerator))]
        internal void Encoder_should_encode_non_wrapped_dynamic(object content, Type contentType, Type dynamicType)
        {
            var dynamicBlock = BlockBuilder.BuildBlock(dynamicType);
            var encoded = Encoder.EncodeBlock(dynamicBlock, content);

            var block = BlockBuilder.BuildBlock(contentType);
            var decoded = Decoder.DecodeBlock(encoded, block, new());

            Assert.Equal(content, decoded);
        }

        [Theory]
        [ClassData(typeof(DynamicSchemasGenerator.Wrapped))]
        internal void Encoder_should_encode_wrapped_dynamic(object content, Type contentType, Type dynamicType)
        {
            var dynamicBlock = BlockBuilder.BuildBlock(dynamicType);
            var encoded = Encoder.EncodeBlock(dynamicBlock, content);

            var block = BlockBuilder.BuildBlock(contentType);
            var decoded = Decoder.DecodeBlock(encoded, block, new());

            Assert.Equal(content, decoded);
        }

        [Theory]
        [ClassData(typeof(SchemasGenerator))]
        internal void Encoder_should_encode_non_wrapped(object content, Type contentType)
        {
            var block = BlockBuilder.BuildBlock(contentType);
            var bytes = Encoder.EncodeBlock(block, content);
            var result = Decoder.DecodeBlock(bytes, block, new());
            Assert.Equal(content, result);
        }

        [Theory]
        [ClassData(typeof(SchemasGenerator.Wrapped))]
        internal void Encoder_should_encode_wrapped(object content, Type contentType)
        {
            var block = BlockBuilder.BuildBlock(contentType);
            var bytes = Encoder.EncodeBlock(block, content);
            var result = Decoder.DecodeBlock(bytes, block, new());
            Assert.Equal(content, result);
        }

        internal class SchemasGeneratorBase : TheoryData
        {
            protected virtual bool OnlyWrappedOrOnlyNonWrapped { get; }
            protected Type CurrentDynamicType { get; private set; } = null!;

            public SchemasGeneratorBase()
            {
                // dynamic
                Add<object>(default(string)!);
                Add<ITerraformValue>(TerraformValue.OfUnknown<object>(), notWrappable: true);

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

            protected void Add<T>(T value, bool isNested = false, bool notWrappable = false)
                where T : notnull
            {
                if (!OnlyWrappedOrOnlyNonWrapped) {
                    var schema = new SchemaFake<T>(value) { IsNestedBlock = isNested }.Schema;
                    CurrentDynamicType = new SchemaFake<object>(value) { IsNestedBlock = isNested }.Schema.GetType();
                    Add(schema, schema.GetType());
                }

                if (OnlyWrappedOrOnlyNonWrapped && !notWrappable) {
                    var schema = new SchemaFake<T>.TerraformValueFake(TerraformValue.OfValue(value)) { IsNestedBlock = isNested }.Schema;
                    CurrentDynamicType = new SchemaFake<object>.TerraformValueFake(TerraformValue.OfValue((object)value)) { IsNestedBlock = isNested }.Schema.GetType();
                    Add(schema, schema.GetType());
                }
            }
        }

        internal class SchemasGenerator : SchemasGeneratorBase
        {
            internal class Wrapped : SchemasGenerator
            {
                protected override bool OnlyWrappedOrOnlyNonWrapped => true;
            }
        }

        internal class DynamicSchemasGenerator : SchemasGeneratorBase
        {
            protected override void Add(params object[] row) =>
                AddRow(row.Concat(new[] { CurrentDynamicType }).ToArray());

            internal class Wrapped : DynamicSchemasGenerator
            {
                protected override bool OnlyWrappedOrOnlyNonWrapped => true;
            }
        }

        private class NonRequestableServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType) => throw new NotImplementedException();
        }
    }
}
