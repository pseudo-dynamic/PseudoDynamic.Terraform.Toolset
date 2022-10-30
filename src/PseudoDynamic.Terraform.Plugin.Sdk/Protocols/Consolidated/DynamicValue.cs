namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal class DynamicValue
    {
        internal static DynamicValue OfMessagePack(ReadOnlyMemory<byte> bytes) =>
            new DynamicValue() { Msgpack = bytes };

        public ReadOnlyMemory<byte> Msgpack { get; set; }
        public ReadOnlyMemory<byte> Json { get; set; }
    }
}
