namespace PseudoDynamic.Terraform.Plugin.Internals.Diagnostics
{
    /// <summary>
    /// An exception that indicates a non-zero code.
    /// </summary>
    public class NonZeroExitCodeException : Exception
    {
        /// <summary>
        /// The non-zero exit code.
        /// </summary>
        public int ExitCode { get; }

        /// <inheritdoc/>
        public NonZeroExitCodeException()
        {
        }

        /// <inheritdoc/>
        public NonZeroExitCodeException(string? message)
          : base(message)
        {
        }

        /// <inheritdoc/>
        public NonZeroExitCodeException(string? message, Exception? innerException)
          : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        public NonZeroExitCodeException(int exitCode) => ExitCode = exitCode;

        /// <inheritdoc/>
        public NonZeroExitCodeException(int exitCode, string? message)
          : base(message)
        {
            ExitCode = exitCode;
        }

        /// <inheritdoc/>
        public NonZeroExitCodeException(int exitCode, string? message, Exception? innerException)
          : base(message, innerException)
        {
            ExitCode = exitCode;
        }
    }
}
