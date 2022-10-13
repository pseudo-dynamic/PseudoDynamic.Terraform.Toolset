namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex
{
    internal interface IVisitedComplexTypes
    {
        IReadOnlySet<Type> VisitedComplexTypes { get; }

        void AddVisitedComplexType(Type type);
    }
}
