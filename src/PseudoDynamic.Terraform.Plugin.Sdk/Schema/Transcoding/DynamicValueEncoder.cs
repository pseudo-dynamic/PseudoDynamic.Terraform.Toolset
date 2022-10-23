using DotNext.Buffers;
using MessagePack;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Schema.Transcoding
{
    internal class DynamicValueEncoder
    {
        public void EncodeSchema(ref MessagePackWriter writer, BlockDefinition block, object schema)
        {
        }

        public ReadOnlyMemory<byte> EncodeSchema(BlockDefinition block, object schema)
        {
            using var bufferWriter = new SparseBufferWriter<byte>(); // TODO: estimate proper defaults
            var writer = new MessagePackWriter(bufferWriter);
            EncodeSchema(ref writer, block, schema);
            var buffer = new byte[bufferWriter.WrittenCount];
            bufferWriter.CopyTo(buffer);
            return buffer;
        }
    }
}
