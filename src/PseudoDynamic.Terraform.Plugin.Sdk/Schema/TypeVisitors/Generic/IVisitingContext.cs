namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal interface IVisitingContext : IVisitorContext
    {
        VisitingContextType ContextType { get; }
        TypeVisitor.VisitingType VisitingType { get; }
    }
}
