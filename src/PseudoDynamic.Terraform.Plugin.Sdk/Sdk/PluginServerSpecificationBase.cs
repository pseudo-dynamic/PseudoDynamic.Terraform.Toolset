using PseudoDynamic.Terraform.Plugin.Protocols;
using PseudoDynamic.Terraform.Plugin.Sdk.Features;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract record PluginServerSpecificationBase<TServerSpecification, TProviderFeatures> : IPluginServerSpecification
        where TServerSpecification : PluginServerSpecificationBase<TServerSpecification, TProviderFeatures>
        where TProviderFeatures : ProviderFeaturesBase
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
        public bool IsDebuggable { get; init; }

        internal (string ProviderName, List<Action<TProviderFeatures>> Delegates)? ProviderConfigurations { get; private set; }

        internal PluginServerSpecificationBase()
        {
        }

        public TServerSpecification ConfigureProvider(string providerName, Action<TProviderFeatures> configureProvider)
        {
            ProviderConfigurations = (providerName, ProviderConfigurations?.Delegates ?? new List<Action<TProviderFeatures>>());
            ProviderConfigurations.Value.Delegates.Add(configureProvider);
            return (TServerSpecification)this;
        }
    }
}
