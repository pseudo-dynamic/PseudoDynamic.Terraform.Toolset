using System.Collections.Specialized;
using System.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    /// <summary>
    /// The start info of a process.
    /// </summary>
    public class SimpleProcessStartInfo
    {
        /// <summary>
        /// The executable to start.
        /// </summary>
        public string? Executable => ProcessStartInfo.FileName;

        /// <summary>
        /// The process arguments.
        /// </summary>
        public string? Arguments => ProcessStartInfo.Arguments;

        public StringDictionary EnvironmentVariables => ProcessStartInfo.EnvironmentVariables;

        internal ProcessStartInfo ProcessStartInfo { get; private set; }

        /// <summary>
        /// Creates an instance of type <see cref="SimpleProcessStartInfo" />.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="workingDirectory">The working directory is not used to find the executable. Instead, its value applies to the process that is started and only has meaning within the context of the new process.</param>
        public SimpleProcessStartInfo(string name, string? args = null, string? workingDirectory = null)
        {
            workingDirectory ??= string.Empty;

            if (args == null)
            {
                ProcessStartInfo = new ProcessStartInfo(name)
                {
                    WorkingDirectory = workingDirectory
                };
            }
            else
            {
                ProcessStartInfo = new ProcessStartInfo(name, args)
                {
                    WorkingDirectory = workingDirectory
                };
            }

            ProcessStartInfo.UseShellExecute = false;
            ProcessStartInfo.RedirectStandardOutput = true;
            ProcessStartInfo.RedirectStandardError = true;
            ProcessStartInfo.CreateNoWindow = true;
        }
    }
}
