namespace PseudoDynamic.Terraform.Plugin
{
    /// <summary>
    /// The kinds of reports.
    /// </summary>
    public enum ReportKind
    {
        /// <summary>
        /// Represents a warning report kind leading Terraform into printing a warning.
        /// </summary>
        Warning,
        /// <summary>
        /// Represents an error report kind leading Terraform into failing after finishing the current remote procedure call initiated by Terraform.
        /// </summary>
        Error
    }
}
