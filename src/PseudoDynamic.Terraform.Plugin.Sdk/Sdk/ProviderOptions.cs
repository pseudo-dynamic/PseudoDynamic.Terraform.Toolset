using System.Diagnostics.CodeAnalysis;

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

        public List<ResourceDescriptor> ResourceDescriptors { get; } = new List<ResourceDescriptor>();

        public string FullyQualifiedProviderName {
            get {
                var providerName = _fullyQualifiedProviderName;
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
