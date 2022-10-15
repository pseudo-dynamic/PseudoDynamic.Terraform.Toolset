using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class ConfigureProvider
    {
        public class Request
        {
            public string? TerraformVersion { get; set; }
            public DynamicValue? Config { get; set; }
        }

        public class Response
        {
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
