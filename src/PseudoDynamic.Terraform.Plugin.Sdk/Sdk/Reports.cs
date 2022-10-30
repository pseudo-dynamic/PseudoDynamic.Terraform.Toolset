using System.Collections;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// A collection of reports.
    /// </summary>
    public class Reports : IEnumerable<Report>
    {
        /// <summary>
        /// The total number of warnings in the collection.
        /// </summary>
        public int TotalWarnings { get; private set; }

        /// <summary>
        /// The total number of errors in the collection.
        /// </summary>
        public int TotalErrors { get; private set; }

        /// <summary>
        /// <see langword="true"/> if any errors have been recorded.
        /// </summary>
        public bool HavingErrors => TotalErrors > 0;

        /// <summary>
        /// <see langword="true"/> if any warnings have been recorded.
        /// </summary>
        public bool HavingWarnings => TotalErrors > 0;

        private IList<Report> _reportCollection { get; } = new List<Report>();

        internal void Report(Report report)
        {
            if (report.Kind == ReportKind.Error) {
                TotalErrors++;
            } else if (report.Kind == ReportKind.Warning) {
                TotalWarnings++;
            }

            _reportCollection.Add(report);
        }

        /// <summary>
        /// Stores a kind of report.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void Report(ReportKind kind, string header, string body) =>
            Report(new Report(kind, header, body));

        /// <summary>
        /// Stores a kind of report.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="header"></param>
        public void Report(ReportKind kind, string header) =>
            Report(kind, header, string.Empty);

        /// <summary>
        /// Stores a warning report leading Terraform into printing a warning.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void Warning(string header, string body) =>
            Report(ReportKind.Warning, header, body);

        /// <summary>
        /// Stores a warning report leading Terraform into printing a warning.
        /// </summary>
        /// <param name="header"></param>
        public void Warning(string header) =>
            Warning(header, string.Empty);

        /// <summary>
        /// Stores an error report leading Terraform into failing after finishing the current remote procedure call initiated by Terraform.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void Error(string header, string body) =>
            Report(ReportKind.Error, header, body);

        /// <summary>
        /// Stores an error report leading Terraform into failing after finishing the current remote procedure call initiated by Terraform.
        /// </summary>
        /// <param name="header"></param>
        public void Error(string header) =>
            Error(header, string.Empty);

        /// <summary>
        /// Stores an error report leading Terraform into failing after finishing the current remote procedure call initiated by Terraform.
        /// </summary>
        /// <param name="error"></param>
        public void Error(Exception error) =>
            Error(error.Message, error.ToString());

        IEnumerator<Report> IEnumerable<Report>.GetEnumerator() => _reportCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Report>)this).GetEnumerator();
    }
}
