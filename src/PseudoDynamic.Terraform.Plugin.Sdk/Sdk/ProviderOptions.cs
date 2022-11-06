using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderOptions
    {
        private static void ValidateProviderName([NotNull] string? providerName)
        {
            if (string.IsNullOrEmpty(providerName)) {
                throw new TerraformNameConventionException("Fully-qualified provider name is null or empty");
            }

            TerraformNameConventionException.EnsureProviderNameConvention(providerName);
        }

        public ProviderServiceDescriptor? ProviderDescriptor { get; set; }

        public Type? ProviderSchemaType { get; set; }

        public List<ResourceServiceDescriptor> ResourceDescriptors { get; } = new List<ResourceServiceDescriptor>();

        public List<DataSourceServiceDescriptor> DataSourceDescriptors { get; } = new List<DataSourceServiceDescriptor>();

        public string FullyQualifiedProviderName {
            get {
                string? providerName = _fullyQualifiedProviderName;
                ValidateProviderName(providerName);
                return providerName;
            }

            set {
                ValidateProviderName(value);
                _fullyQualifiedProviderName = value;
            }
        }

        private string? _fullyQualifiedProviderName;
    }
}
