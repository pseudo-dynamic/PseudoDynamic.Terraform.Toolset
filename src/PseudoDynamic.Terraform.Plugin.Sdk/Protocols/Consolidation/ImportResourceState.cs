namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidation
{
    internal static class ImportResourceState
    {
        public class Request
        {
            public string TypeName {
                get => _typeName ?? throw new InvalidOperationException("Type name is unexpectedly null");
                set => _typeName = value;
            }

            public string? Id { get; set; }

            private string? _typeName;
        }

        public class ImportedResource
        {
            public string? TypeName { get; set; }
            public DynamicValue? State { get; set; }
            public ReadOnlyMemory<byte> Private { get; set; }
        }

        public class Response
        {
            public IList<ImportedResource>? ImportedResources { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
