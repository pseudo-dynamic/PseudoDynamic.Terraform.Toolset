namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ProviderResourceDefinition : ResourceDefinition
    {
        public string ResourceTypeName { get; }

        public ProviderResourceDefinition(ResourceDefinition original, string resourceTypeName)
            : base(original) =>
            ResourceTypeName = resourceTypeName;
    }
}
