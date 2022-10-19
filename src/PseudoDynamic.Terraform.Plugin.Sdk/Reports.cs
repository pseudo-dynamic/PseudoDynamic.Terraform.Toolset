using System.Collections;

namespace PseudoDynamic.Terraform.Plugin
{
    public class Reports : IEnumerable<Report>
    {
        private IList<Report> _reportCollection { get; } = new List<Report>();

        internal void Report(Report report) =>
            _reportCollection.Add(report);

        public void Report(ReportKind kind, string header, string body = "") =>
            Report(new Report(kind, header, body));

        public void Warn(string header, string body = "") =>
            Report(ReportKind.Warning, header, body);

        public void Error(string header, string body = "") =>
            Report(ReportKind.Error, header, body);

        IEnumerator<Report> IEnumerable<Report>.GetEnumerator() => _reportCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Report>)this).GetEnumerator();
    }
}
