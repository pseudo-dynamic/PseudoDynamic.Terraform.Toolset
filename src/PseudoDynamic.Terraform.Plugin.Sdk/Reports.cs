using System.Collections;

namespace PseudoDynamic.Terraform.Plugin
{
    /// <summary>
    /// A collection of reports.
    /// </summary>
    public class Reports : IEnumerable<Report>
    {
        private IList<Report> _reportCollection { get; } = new List<Report>();

        internal void Report(Report report) =>
            _reportCollection.Add(report);

        /// <summary>
        /// Stores a kind of report.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void Report(ReportKind kind, string header, string body = "") =>
            Report(new Report(kind, header, body));

        /// <summary>
        /// Stores a warning report leading Terraform into printing a warning.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void Warning(string header, string body = "") =>
            Report(ReportKind.Warning, header, body);

        /// <summary>
        /// Stores an error report leading Terraform into failing after finishing the current remote procedure call initiated by Terraform.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void Error(string header, string body = "") =>
            Report(ReportKind.Error, header, body);

        IEnumerator<Report> IEnumerable<Report>.GetEnumerator() => _reportCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Report>)this).GetEnumerator();
    }
}
