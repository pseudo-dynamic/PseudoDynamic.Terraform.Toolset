namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal static class ConfigureProvider
    {
        public class Request
        {

            public string? TerraformVersion { get; set; }

            public DynamicValue Config {
                get => _config ?? throw new InvalidOperationException("Config is unexpectedly null");
                set => _config = value;
            }

            private DynamicValue? _config;
        }

        public class Response
        {
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
