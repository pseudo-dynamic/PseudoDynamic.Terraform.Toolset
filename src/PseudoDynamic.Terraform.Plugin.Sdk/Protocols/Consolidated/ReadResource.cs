namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal static class ReadResource
    {
        public class Request
        {
            public string TypeName {
                get => _typeName ?? throw new InvalidOperationException("Type name is unexpectedly null");
                set => _typeName = value;
            }

            public DynamicValue CurrentState {
                get => _currentState ?? throw new InvalidOperationException("Current state is unexpectedly null");
                set => _currentState = value;
            }

            public ReadOnlyMemory<byte> Private { get; set; }

            public DynamicValue ProviderMeta {
                get => _providerMeta ?? throw new InvalidOperationException("Provider meta is unexpectedly null");
                set => _providerMeta = value;
            }

            private string? _typeName;
            private DynamicValue? _currentState;
            private DynamicValue? _providerMeta;
        }

        public class Response
        {
            public DynamicValue? NewState { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
            public ReadOnlyMemory<byte> Private { get; set; }
        }
    }
}
