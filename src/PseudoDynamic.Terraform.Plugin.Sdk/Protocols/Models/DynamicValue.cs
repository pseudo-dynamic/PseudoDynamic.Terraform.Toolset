namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal class DynamicValue
    {
        public ReadOnlyMemory<byte> Msgpack { get; set; }
        public ReadOnlyMemory<byte> Json { get; set; }
    }
}
