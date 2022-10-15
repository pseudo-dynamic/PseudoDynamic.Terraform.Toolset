using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal class RawState
    {
        public IEnumerable<byte>? Json { get; set; }
        public IDictionary<string, string>? Flatmap { get; set; }
    }
}
