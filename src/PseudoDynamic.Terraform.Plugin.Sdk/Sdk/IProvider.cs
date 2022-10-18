namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal interface IProvider
    {
        string FullyQualifiedProviderName { get; }
        string ProviderName { get; }
        IReadOnlyDictionary<string, ResourceDefinition> ResourceDefinitions { get; }
    }
}
