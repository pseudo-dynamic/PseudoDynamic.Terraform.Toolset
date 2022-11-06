using System.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics;

internal interface IAsyncSimpleProcess
{
    /// <inheritdoc cref="Process.Start()" />
    bool Start();

    /// <inheritdoc cref="Process.WaitForExitAsync(CancellationToken)" />
    Task<int> WaitForExitAsync();
}
