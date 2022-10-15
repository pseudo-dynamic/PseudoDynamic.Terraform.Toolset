namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal class DynamicValue
    {
        public IEnumerable<byte>? Msgpack { get; set; }
        public IEnumerable<byte>? Json { get; set; }
    }
}
