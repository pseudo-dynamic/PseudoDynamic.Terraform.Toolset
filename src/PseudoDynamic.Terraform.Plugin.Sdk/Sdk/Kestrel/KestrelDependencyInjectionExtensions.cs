using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Kestrel
{
    internal static class KestrelDependencyInjectionExtensions
    {
        public static IServiceCollection AddKestrelLoopbackListener(this IServiceCollection services)
        {
            if (!services.Any(x => x.ImplementationType == typeof(KestrelLoopbackListenerConfiguration))) {
                services.AddSingleton<IPostConfigureOptions<KestrelServerOptions>, KestrelLoopbackListenerConfiguration>();
            }

            return services;
        }
    }
}
