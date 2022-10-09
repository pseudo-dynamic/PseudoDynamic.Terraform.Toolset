using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoDynamic.Terraform.Plugin.Schema.Conventions
{
    internal interface INameConvention
    {
        public string Format(string name);
    }
}
