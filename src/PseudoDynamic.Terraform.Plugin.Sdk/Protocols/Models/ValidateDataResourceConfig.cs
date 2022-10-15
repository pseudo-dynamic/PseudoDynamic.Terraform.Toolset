namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class ValidateDataResourceConfig
    {
        public class Request
        {
            public string? TypeName { get; set; }
            public DynamicValue? Config { get; set; }
        }

        public class Response
        {
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
