namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidation
{
    internal static class ReadDataSource
    {
        public class Request
        {
            public string? TypeName { get; set; }
            public DynamicValue? Config { get; set; }
            public DynamicValue? ProviderMeta { get; set; }
        }

        public class Response
        {
            public DynamicValue? State { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
