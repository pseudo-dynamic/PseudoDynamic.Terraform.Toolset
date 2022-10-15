using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class StopProvider
    {
        public class Request
        {
        }

        public class Response
        {
            public string? Error { get; set; }
        }
    }
}
