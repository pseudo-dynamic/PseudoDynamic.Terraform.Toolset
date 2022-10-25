namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidation
{
    internal static class ValidateDataResourceConfig
    {
        public class Request
        {
            public string TypeName {
                get => _typeName ?? throw new InvalidOperationException("Type name is unexpectedly null");
                set => _typeName = value;
            }

            public DynamicValue? Config { get; set; }

            private string? _typeName;
        }

        public class Response
        {
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
