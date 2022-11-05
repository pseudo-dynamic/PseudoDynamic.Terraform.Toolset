using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <summary>
    /// An exception that indicates a non-zero code.
    /// </summary>
    public class BadExitCodeException : Exception
    {
        /// <summary>
        /// The bad exit code.
        /// </summary>
        public int? ExitCode { get; init; }

        /// <inheritdoc/>
        public override string Message => (base.Message == string.Empty ? $"The process exited with bad exit code" : base.Message)
           + (ExitCode != null ? $"{Environment.NewLine}Exit Code = {ExitCode}" : string.Empty);

        /// <inheritdoc/>
        public BadExitCodeException()
        {
        }

        /// <inheritdoc/>
        public BadExitCodeException(string? message) : base(message)
        {
        }

        /// <inheritdoc/>
        public BadExitCodeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        protected BadExitCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
