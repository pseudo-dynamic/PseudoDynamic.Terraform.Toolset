using System.Diagnostics.CodeAnalysis;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal interface ITerraformValue
    {
        object? Value { get; }

        [MemberNotNullWhen(false, nameof(Value))]
        bool IsNull { get; }

        bool IsUnknown { get; }
    }
}
