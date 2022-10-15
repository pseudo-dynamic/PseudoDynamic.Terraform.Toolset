using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class ReadDataSource
    {
        public class Request
        {
            public string? TypeName { get; set; }
            public DynamicValue? Config { get; set; }
            public DynamicValue? ProviderMeta { get; set; }
        }

        public class Response
        {
            public DynamicValue? State { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
