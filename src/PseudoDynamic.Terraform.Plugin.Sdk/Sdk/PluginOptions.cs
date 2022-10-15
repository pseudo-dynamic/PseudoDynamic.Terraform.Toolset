namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class ProviderOptions
    {
        public List<ResourceDescriptor> ResourceDescriptors { get; } = new List<ResourceDescriptor>();

        public string FullyQualifiedProviderName {
            get => _providerName ?? throw new InvalidOperationException("Fully-qualified provider name was not set");
            set => _providerName = value;
        }

        /// <summary>
        /// The name convention. (default snake_case)
        /// </summary>
        public INameConvention NameConvention {
            get => _nameConvention ?? SnakeCaseConvention.Default;
            set => _nameConvention = value;
        }

        private INameConvention? _nameConvention;
        private string? _providerName;
    }
}
