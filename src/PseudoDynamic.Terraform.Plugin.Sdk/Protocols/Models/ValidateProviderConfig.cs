namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class ValidateProviderConfig
    {
        public class Request
        {
            public DynamicValue? Config { get; set; }
        }
        public class Response
        {
            public DynamicValue? PreparedConfig { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
