using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeSystem
{
    internal class DictionaryDefinition : MonoRangeDefinition
    {
        public override TerraformType TerraformType => TerraformType.Map;
    }
}
