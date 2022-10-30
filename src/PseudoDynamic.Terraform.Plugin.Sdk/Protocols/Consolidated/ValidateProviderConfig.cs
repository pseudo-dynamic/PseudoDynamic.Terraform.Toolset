namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal static class ValidateProviderConfig
    {
        public class Request
        {
            public DynamicValue Config {
                get => _config ?? throw new InvalidOperationException("Config is unexpectedly null");
                set => _config = value;
            }

            private DynamicValue? _config;
        }
        public class Response
        {
            public DynamicValue? PreparedConfig { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
