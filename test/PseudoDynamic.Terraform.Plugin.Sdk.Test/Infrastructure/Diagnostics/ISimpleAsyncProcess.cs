namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <summary>
    /// An async process.
    /// </summary>
    public interface ISimpleAsyncProcess : IDisposable
    {
        /// <summary>
        /// Indicates whether the process has been started.
        /// </summary>
        bool IsProcessStarted { get; }

        /// <summary>
        /// Starts the process.
        /// </summary>
        void Start();

        /// <summary>
        /// Waits for exit asynchronously.
        /// </summary>
        /// <returns>The exit code.</returns>
        Task<int> WaitForExitAsync();

        /// <summary>
        /// Kills the process.
        /// </summary>
        void Kill();
    }
}
