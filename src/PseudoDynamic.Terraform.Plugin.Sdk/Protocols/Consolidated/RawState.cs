namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal class RawState
    {
        public ReadOnlyMemory<byte> Json { get; set; }
        public IDictionary<string, string>? Flatmap { get; set; }
    }
}
