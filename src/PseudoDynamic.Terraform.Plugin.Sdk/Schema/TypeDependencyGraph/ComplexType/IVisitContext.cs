namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitContext : IContext
    {
        VisitContextType ContextType { get; }
        Type VisitType { get; }

        T? GetVisitedTypeAttribute<T>() where T : Attribute;
        T? GetContextualAttribute<T>() where T : Attribute;
    }
}
