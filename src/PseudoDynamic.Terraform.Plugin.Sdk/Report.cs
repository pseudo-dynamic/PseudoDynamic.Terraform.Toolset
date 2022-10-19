namespace PseudoDynamic.Terraform.Plugin
{
    internal class Report
    {
        public ReportKind Kind { get; }
        public string Header { get; }
        public string Body { get; }

        internal Report(ReportKind kind, string header, string body)
        {
            Kind = kind;
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }
    }
}
