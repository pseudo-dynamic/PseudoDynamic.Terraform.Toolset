using FluentAssertions;
using MessagePack;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    public class TerraformDynamicMessagePackEncoderGenericsTests
    {
        [Fact]
        internal void Generic_encode_value_inferres_correct_content_type()
        {
            var encoder = new ContentTypeEvaluatingEncoder();
            encoder.EncodeValue(PrimitiveDefinition.Number, default(int));
            encoder.ContentType.Should().Be(typeof(int));
        }

        [Fact]
        internal void Generic_encode_value_can_handle_null()
        {
            var encoder = new ContentTypeEvaluatingEncoder();
            encoder.EncodeValue<object?>(DynamicDefinition.Uncomputed(), default);
            encoder.ContentType.Should().Be(typeof(object));
        }

        [Fact]
        internal void Non_generic_encode_value_inferres_correct_content_type()
        {
            var encoder = new ContentTypeEvaluatingEncoder();
            var number = 2;
            encoder.EncodeValue(PrimitiveDefinition.Number, (object?)number);
            encoder.ContentType.Should().Be(typeof(int));
        }

        [Fact]
        internal void Non_generic_encode_value_can_handle_null()
        {
            var encoder = new ContentTypeEvaluatingEncoder();
            encoder.EncodeValue(DynamicDefinition.Uncomputed(), default);
            encoder.ContentType.Should().Be(typeof(object));
        }

        class ContentTypeEvaluatingEncoder : TerraformDynamicMessagePackEncoder
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
