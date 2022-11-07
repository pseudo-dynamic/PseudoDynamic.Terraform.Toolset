using MessagePack;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    internal interface ITerraformValueMessagePackEncoder
    {
        void Encode<T>(ref MessagePackWriter writer, ValueDefinition value, ITerraformValue<T> terraformValue);
        void Encode(ref MessagePackWriter writer, ValueDefinition value, ITerraformValue terraformValue);
    }
}
