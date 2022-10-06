namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal interface IVisitContext : IRootVisitContext
    {
        VisitContextType ContextType { get; }
        Type VisitedType { get; }
    }
}
