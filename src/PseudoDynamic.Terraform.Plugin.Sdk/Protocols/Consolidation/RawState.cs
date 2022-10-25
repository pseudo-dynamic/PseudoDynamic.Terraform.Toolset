namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidation
{
    internal class RawState
    {
        public ReadOnlyMemory<byte> Json { get; set; }
        public IDictionary<string, string>? Flatmap { get; set; }
    }
}
