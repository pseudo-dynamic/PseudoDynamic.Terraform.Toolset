using System.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics
{
    internal interface ISimpleProcess : IDisposable
    {
        /// <inheritdoc cref="Process.Start()" />
        bool Start();

        /// <inheritdoc cref="Process.WaitForExit()" />
        int WaitForExit();
    }
}
