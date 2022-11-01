using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk.Features;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IPluginServerSpecification
    {
        public static ProtocolV5 NewProtocolV5() => new ProtocolV5();
        public static ProtocolV6 NewProtocolV6() => new ProtocolV6();

        /// <summary>
        /// The intended plugin protocol version to be used by the gRPC server.
        /// </summary>
        PluginProtocol Protocol { get; }

        /// <summary>
        /// By default, HTTPS is enabled, which is required by Terraform in production environment.
        /// However, if this value is set to <see langword="true"/>, HTTP is used instead and the
        /// instructions for Terraform using this debug instance are displayed in the console output.
        /// </summary>
        bool IsDebuggable { get; }

        public sealed record ProtocolV5 : PluginServerSpecificationBase<ProtocolV5, ProtocolV5.ProviderFeatures>
        {
            public override PluginProtocol Protocol => PluginProtocol.V5;

            public class ProviderFeatures : ProviderFeaturesBase
            {
                public ProviderFeatures(IServiceCollection services) : base(services)
                {
                }
            }
        }

        public sealed record ProtocolV6 : PluginServerSpecificationBase<ProtocolV6, ProtocolV6.ProviderFeatures>
        {
            public override PluginProtocol Protocol => PluginProtocol.V5;

            public class ProviderFeatures : ProviderFeaturesBase
            {
                public ProviderFeatures(IServiceCollection services) : base(services)
                {
                }
            }
        }
    }
}
