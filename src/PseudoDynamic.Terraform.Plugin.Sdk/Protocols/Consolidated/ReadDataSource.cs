namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal static class ReadDataSource
    {
        public class Request
        {
            public string TypeName {
                get => _typeName ?? throw new InvalidOperationException("Type name is unexpectedly null");
                set => _typeName = value;
            }

            public DynamicValue Config {
                get => _config ?? throw new InvalidOperationException("Config is unexpectedly null");
                set => _config = value;
            }

            public DynamicValue ProviderMeta {
                get => _providerMeta ?? throw new InvalidOperationException("Provider meta is unexpectedly null");
                set => _providerMeta = value;
            }

            private string? _typeName;
            private DynamicValue? _config;
            private DynamicValue? _providerMeta;
        }

        public class Response
        {
            public DynamicValue? State { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
