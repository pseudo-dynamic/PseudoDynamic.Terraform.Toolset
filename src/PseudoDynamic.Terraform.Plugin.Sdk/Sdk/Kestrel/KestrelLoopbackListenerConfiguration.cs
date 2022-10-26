using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.X509;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Kestrel
{
    internal class KestrelLoopbackListenerConfiguration : IPostConfigureOptions<KestrelServerOptions>, IPostConfigureOptions<PluginServerOptions>
    {
        private readonly IServiceProvider _serviceProvider;
        private X509Certificate2? _selfSignedCertificate;

        public KestrelLoopbackListenerConfiguration(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        private X509Certificate2 CreateSelfSignedCertificate()
        {
            var rsaKeyPair = Certificate.BouncyCastle.GenerateRsaKeyPair(2048);
            var issuer = new X509Name("CN=127.0.0.1");
            var subject = new X509Name("CN=pseudo-dynamic");
            var subjectAltName = new GeneralNames(new GeneralName(GeneralName.DnsName, "localhost"));
            var bouncyCastleCertificate = Certificate.BouncyCastle.GenerateSelfSignedCertificate(issuer, subject, rsaKeyPair.Private, rsaKeyPair.Public, subjectAltName);
            return Certificate.GenerateSelfSignedCertificate(bouncyCastleCertificate, rsaKeyPair.Private);
        }

        private X509Certificate2 GetSelfSignedCertificate() =>
            _selfSignedCertificate ?? CreateSelfSignedCertificate();

        public void PostConfigure(string _, PluginServerOptions options)
        {
            if (!options.IsDebuggable) {
                options.ClientCertificate = GetSelfSignedCertificate();
            }
        }

        public void PostConfigure(string _, KestrelServerOptions options)
        {
            var pluginServerOptions = _serviceProvider.GetService<IOptions<PluginServerOptions>>()?.Value
                ?? throw new InvalidOperationException($"Expected non-null plugin server options");

            var ipAddress = IPAddress.Loopback;
            const int randomPort = 0;

            if (pluginServerOptions.IsDebuggable) {
                options.Listen(ipAddress, randomPort, options => options.Protocols = HttpProtocols.Http2);
            } else {
                var selfSignedCertificate = GetSelfSignedCertificate();

                if (pluginServerOptions.ClientCertificate?.Thumbprint != selfSignedCertificate.Thumbprint) {
                    throw new InvalidOperationException("You should not set the client certificate of the plugin server options, if you are using the Terraform provider defaults of the plugin host builder");
                }

                options.Listen(ipAddress, randomPort, listen => listen.UseHttps(http => {
                    http.ServerCertificate = selfSignedCertificate;
                    http.AllowAnyClientCertificate();
                }));
            }
        }
    }
}
