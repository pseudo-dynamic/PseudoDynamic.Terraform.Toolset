using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract record PluginServerSpecificationBase<TDerived> : IPluginServerSpecification
        where TDerived : PluginServerSpecificationBase<TDerived>
    {
        /// <summary>
        /// The intended plugin protocol version to be used by the gRPC server.
        /// </summary>
        public abstract PluginProtocol Protocol { get; }

        /// <summary>
        /// By default, HTTPS is enabled, which is required by Terraform in production environment.
        /// However, if this value is set to <see langword="true"/>, HTTP is used instead and the
        /// instructions for Terraform using this debug instance are displayed in the console output.
        /// </summary>
        public bool IsDebuggable { get; set; }

        internal (string ProviderName, Type? ProviderMetaSchemaType, Action<IServiceCollection> ConfigureProvider)? ProviderConfiguration { get; set; }

        internal PluginServerSpecificationBase()
        {
        }

        public TDerived Debuggable(bool isDebuggable = true)
        {
            IsDebuggable = isDebuggable;
            return (TDerived)this;
        }

        protected TDerived SetProvider<TProviderFeatures>(
            string providerName,
            Action<TProviderFeatures> configureProvider,
            Func<IServiceCollection, TProviderFeatures> providerFeaturesFactory,
            Type? providerMetaSchemaType)
            where TProviderFeatures : IProviderFeature
        {
            ProviderConfiguration = (providerName, providerMetaSchemaType, (IServiceCollection services) => configureProvider(providerFeaturesFactory(services)));
            return (TDerived)this;
        }
    }
}
