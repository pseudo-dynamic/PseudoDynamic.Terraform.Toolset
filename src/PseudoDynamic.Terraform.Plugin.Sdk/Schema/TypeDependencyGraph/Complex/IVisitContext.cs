namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex
{
    internal interface IVisitContext : IContext
    {
        VisitContextType ContextType { get; }
        Type VisitedType { get; }
    }
}
