namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class ReadResource
    {
        public class Request
        {
            public string? TypeName { get; set; }
            public DynamicValue? CurrentState { get; set; }
            public IEnumerable<byte>? Private { get; set; }
            public DynamicValue? ProviderMeta { get; set; }
        }

        public class Response
        {
            public DynamicValue? NewState { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
            public IEnumerable<byte>? Private { get; set; }
        }
    }
}
