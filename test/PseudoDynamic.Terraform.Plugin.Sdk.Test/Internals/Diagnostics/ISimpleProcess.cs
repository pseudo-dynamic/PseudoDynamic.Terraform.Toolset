namespace PseudoDynamic.Terraform.Plugin.Internals.Diagnostics
{
    /// <summary>
    /// A process.
    /// </summary>
    public interface ISimpleProcess : IDisposable
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
        /// Waits for exit.
        /// </summary>
        /// <returns>The exit code.</returns>
        int WaitForExit();

        /// <summary>
        /// Kills the process.
        /// </summary>
        void Kill();
    }
}
