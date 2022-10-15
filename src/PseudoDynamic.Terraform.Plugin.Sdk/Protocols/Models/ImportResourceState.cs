using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class ImportResourceState
    {
        public class Request
        {
            public string? TypeName { get; set; }
            public string? Id { get; set; }
        }

        public class ImportedResource
        {
            public string? TypeName { get; set; }
            public DynamicValue? State { get; set; }
            public IEnumerable<byte>? Private { get; set; }
        }

        public class Response
        {
            public IList<ImportedResource>? ImportedResources { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
