using Microsoft.Extensions.Hosting;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class HostedService<T> : IHostedService
    {
        public HostedService(T _)
        {
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
