namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Assumes that the implementation has a nested value.
    /// This applies to all range type constraints.
    /// </summary>
    internal interface INestedValueAccessor
    {
        ValueDefinition NestedValue { get; }
    }
}
