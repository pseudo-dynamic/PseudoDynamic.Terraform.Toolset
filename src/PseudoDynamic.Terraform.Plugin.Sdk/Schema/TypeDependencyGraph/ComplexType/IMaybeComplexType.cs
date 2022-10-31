namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IMaybeComplexType
    {
        Type Type { get; }
        bool IsComplexType { get; }
    }
}
