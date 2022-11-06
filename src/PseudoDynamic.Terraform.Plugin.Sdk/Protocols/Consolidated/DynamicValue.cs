namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal class DynamicValue
    {
        internal static DynamicValue OfMessagePack(ReadOnlyMemory<byte> bytes) =>
            new() { Msgpack = bytes };

        internal static DynamicValue OfJson(ReadOnlyMemory<byte> bytes) =>
            new() { Json = bytes };

        public ReadOnlyMemory<byte> Msgpack { get; set; }
        public ReadOnlyMemory<byte> Json { get; set; }
    }
}
