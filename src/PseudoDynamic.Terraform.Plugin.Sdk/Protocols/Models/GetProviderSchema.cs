using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class GetProviderSchema
    {
        public class Request
        {
        }

        public class Response
        {
            public BlockDefinition? Provider { get; set; }
            public IDictionary<string, BlockDefinition>? ResourceSchemas { get; set; }
            public IDictionary<string, BlockDefinition>? DataSourceSchemas { get; set; }
            public BlockDefinition? ProviderMeta { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
            public ServerCapabilities? ServerCapabilities { get; set; }
        }

        public class ServerCapabilities
        {
            public bool PlanDestroy { get; set; }
        }
    }
}
