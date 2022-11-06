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
        private static X509Certificate2? _selfSignedCertificate;

        private static X509Certificate2 CreateSelfSignedCertificate()
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair rsaKeyPair = Certificate.BouncyCastle.GenerateRsaKeyPair(2048);
            X509Name issuer = new("CN=127.0.0.1");
            X509Name subject = new("CN=pseudo-dynamic");
            GeneralNames subjectAltName = new(new GeneralName(GeneralName.DnsName, "localhost"));
            Org.BouncyCastle.X509.X509Certificate bouncyCastleCertificate = Certificate.BouncyCastle.GenerateSelfSignedCertificate(issuer, subject, rsaKeyPair.Private, rsaKeyPair.Public, subjectAltName);
            return Certificate.GenerateSelfSignedCertificate(bouncyCastleCertificate, rsaKeyPair.Private);
        }

        private static X509Certificate2 GetSelfSignedCertificate() =>
            _selfSignedCertificate ??= CreateSelfSignedCertificate();

        private readonly IServiceProvider _serviceProvider;

        public KestrelLoopbackListenerConfiguration(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        public void PostConfigure(string _, PluginServerOptions options)
        {
            if (!options.IsDebuggable) {
                options.ClientCertificate = GetSelfSignedCertificate();
            }
        }

        public void PostConfigure(string _, KestrelServerOptions options)
        {
            PluginServerOptions pluginServerOptions = _serviceProvider.GetService<IOptions<PluginServerOptions>>()?.Value
                ?? throw new InvalidOperationException("Expected non-null plugin server options");

            IPAddress ipAddress = IPAddress.Loopback;
            const int randomPort = 0;

            if (pluginServerOptions.IsDebuggable) {
                options.Listen(ipAddress, randomPort, options => options.Protocols = HttpProtocols.Http2);
            } else {
                X509Certificate2 selfSignedCertificate = GetSelfSignedCertificate();

                if (pluginServerOptions.ClientCertificate?.Thumbprint != selfSignedCertificate.Thumbprint) {
                    throw new InvalidOperationException("Do not set the client certificate of the plugin server options manually when using UseTerraformPluginServer()");
                }

                options.Listen(ipAddress, randomPort, listen => listen.UseHttps(http => http.ServerCertificate = selfSignedCertificate));
            }
        }
    }
}
