namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitContext : IContext, IMaybeComplexType
    {
        VisitContextType ContextType { get; }
        Type VisitType { get; }
        IReadOnlySet<TerraformTypeConstraint> ImplicitTypeConstraints { get; }

        T? GetVisitTypeAttribute<T>() where T : Attribute;
        T? GetContextualAttribute<T>() where T : Attribute;
    }
}
