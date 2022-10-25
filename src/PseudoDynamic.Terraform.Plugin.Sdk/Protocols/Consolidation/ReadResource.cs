namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidation
{
    internal static class ReadResource
    {
        public class Request
        {
            public string TypeName {
                get => _typeName ?? throw new InvalidOperationException("Type name is unexpectedly null");
                set => _typeName = value;
            }

            public DynamicValue? CurrentState { get; set; }
            public ReadOnlyMemory<byte> Private { get; set; }
            public DynamicValue? ProviderMeta { get; set; }

            private string? _typeName;
        }

        public class Response
        {
            public DynamicValue? NewState { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
            public ReadOnlyMemory<byte> Private { get; set; }
        }
    }
}
