using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using PseudoDynamic.Terraform.Plugin.Sdk.Services;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal interface IProviderContext
    {
        ProviderService ProviderService { get; }
        BlockDefinition ProviderMetaSchema { get; }
        IReadOnlyDictionary<string, ProviderResourceService> ResourceServices { get; }
        IReadOnlyDictionary<string, ProviderDataSourceService> DataSourceServices { get; }

        internal void ReplaceResource(ResourceServiceDescriptor resourceDescriptor);
    }
}
