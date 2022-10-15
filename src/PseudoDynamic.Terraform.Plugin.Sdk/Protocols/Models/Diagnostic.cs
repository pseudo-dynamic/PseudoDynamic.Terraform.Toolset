namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal class Diagnostic
    {
        public DiagnosticSeverity Severity { get; set; }
        public string? Summary { get; set; }
        public string? Detail { get; set; }
        public AttributePath? Attribute { get; set; }

        internal enum DiagnosticSeverity
        {
            Invalid,
            Error,
            Warning
        }
    }
}
