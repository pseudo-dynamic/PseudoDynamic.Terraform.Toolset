using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal interface IProviderServer
    {
        string FullyQualifiedProviderName { get; }
        string ProviderName { get; }
        string SnakeCaseProviderName { get; }

        internal TerraformReattachProvider TerraformReattachProvider { get; }
    }
}
