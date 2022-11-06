using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MessagePack;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    public class TerraformDynamicMessagePackEncoderGenericsTests
    {
        [Fact]
        internal void Generic_encode_value_inferres_correct_content_type()
        {
            ContentTypeEvaluatingEncoder encoder = new();
            encoder.EncodeValue(PrimitiveDefinition.Number, default(int));
            encoder.ContentType.Should().Be(typeof(int));
        }

        [Fact]
        internal void Generic_encode_value_can_handle_null()
        {
            ContentTypeEvaluatingEncoder encoder = new();
            encoder.EncodeValue<object?>(DynamicDefinition.Uncomputed(), default);
            encoder.ContentType.Should().Be(typeof(object));
        }

        [Fact]
        internal void Non_generic_encode_value_inferres_correct_content_type()
        {
            ContentTypeEvaluatingEncoder encoder = new();
            int number = 2;
            encoder.EncodeValue(PrimitiveDefinition.Number, (object?)number);
            encoder.ContentType.Should().Be(typeof(int));
        }

        [Fact]
        internal void Non_generic_encode_value_can_handle_null()
        {
            ContentTypeEvaluatingEncoder encoder = new();
            encoder.EncodeValue(DynamicDefinition.Uncomputed(), default);
            encoder.ContentType.Should().Be(typeof(object));
        }

        private class ContentTypeEvaluatingEncoder : TerraformDynamicMessagePackEncoder
        {
            public ContentTypeEvaluatingEncoder() : base(new SchemaBuilder())
            {
            }

            public Type? ContentType { get; private set; }

            protected internal override void EncodeValue<T>(ref MessagePackWriter writer, ValueDefinition value, [AllowNull] T content) =>
                ContentType = typeof(T);
        }
    }
}
