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

        public sealed record ProtocolV5 : PluginServerSpecificationBase<ProtocolV5>
        {
            public override PluginProtocol Protocol => PluginProtocol.V5;

            public class ProviderFeatures<ProviderMetaSchema> : ProviderFeaturesBase<ProviderMetaSchema>
            {
                internal ProviderFeatures(IServiceCollection services) : base(services)
                {
                }
            }

            public ProtocolV5 UseProvider<ProviderMetaSchema>(string providerName, Action<ProviderFeatures<ProviderMetaSchema>> configureProvider)
                where ProviderMetaSchema : class =>
                SetProvider(providerName, configureProvider, static services => new ProviderFeatures<ProviderMetaSchema>(services), typeof(ProviderMetaSchema));

            public ProtocolV5 UseProvider(string providerName, Action<ProviderFeatures<object>> configureProvider) =>
                SetProvider(providerName, configureProvider, static services => new ProviderFeatures<object>(services), providerMetaSchemaType: null);
        }

        public sealed record ProtocolV6 : PluginServerSpecificationBase<ProtocolV6>
        {
            public override PluginProtocol Protocol => PluginProtocol.V5;

            public class ProviderFeatures<ProviderMetaSchema> : ProviderFeaturesBase<ProviderMetaSchema>
            {
                internal ProviderFeatures(IServiceCollection services) : base(services)
                {
                }
            }

            public ProtocolV6 UseProvider<ProviderMetaSchema>(string providerName, Action<ProviderFeatures<ProviderMetaSchema>> configureProvider)
                where ProviderMetaSchema : class =>
                SetProvider(providerName, configureProvider, static services => new ProviderFeatures<ProviderMetaSchema>(services), typeof(ProviderMetaSchema));

            public ProtocolV6 UseProvider(string providerName, Action<ProviderFeatures<object>> configureProvider) =>
                SetProvider(providerName, configureProvider, static services => new ProviderFeatures<object>(services), providerMetaSchemaType: null);
        }
    }
}
