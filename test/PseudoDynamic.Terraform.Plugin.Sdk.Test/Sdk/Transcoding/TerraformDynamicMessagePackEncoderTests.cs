using PseudoDynamic.Terraform.Plugin.Infrastructure.Fakes;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using System.Numerics;
using static PseudoDynamic.Terraform.Plugin.Infrastructure.CollectionFactories;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    public class TerraformDynamicMessagePackEncoderTests
    {
        private readonly static TerraformDynamicMessagePackDecoder Decoder = new TerraformDynamicMessagePackDecoder(new NonRequestableServiceProvider());

        [Theory]
        [ClassData(typeof(SchemasGenerator))]
        internal void Encoder_should_encode_then_decode_schemas(object content)
        {
            var block = BlockBuilder.Default.BuildBlock(content.GetType());
            var bytes = TerraformDynamicMessagePackEncoder.Default.EncodeSchema(block, content);
            var result = Decoder.DecodeSchema(bytes, block, new TerraformDynamicMessagePackDecoder.DecodingOptions());
            Assert.Equal(content, result);
        }

        internal class SchemasGenerator : TheoryData
        {
            public SchemasGenerator()
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

            private void Add<T>(T value, bool isNested = false, bool notWrappable = false)
            {
                AddRow(new SchemaFake<T>(value) { IsNestedBlock = isNested }.Schema);

                if (!notWrappable)
                {
                    AddRow(new SchemaFake<T>.TerraformValueFake(TerraformValue.OfValue(value)) { IsNestedBlock = isNested }.Schema);
                }
            }
        }

        private class NonRequestableServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType) => throw new NotImplementedException();
        }
    }
}
