namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitedComplexTypes
    {
        IReadOnlySet<Type> VisitedComplexTypes { get; }

        void AddVisitedComplexType(Type type);
    }
}
