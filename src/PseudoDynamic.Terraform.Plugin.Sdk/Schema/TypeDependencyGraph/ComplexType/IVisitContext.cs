namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitContext : IContext
    {
        VisitContextType ContextType { get; }
        Type VisitedType { get; }
    }
}
