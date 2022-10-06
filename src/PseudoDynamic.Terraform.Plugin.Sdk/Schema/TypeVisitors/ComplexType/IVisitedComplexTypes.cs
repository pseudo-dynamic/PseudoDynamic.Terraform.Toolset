namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal interface IVisitedComplexTypes
    {
        IReadOnlySet<Type> VisitedComplexTypes { get; }

        void AddVisitedComplexType(Type type);
    }
}
