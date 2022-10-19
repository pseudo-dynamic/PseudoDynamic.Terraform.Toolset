namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class RangeDefinition : ValueDefinition
    {
        protected RangeDefinition(Type sourceType) : base(sourceType)
        {
        }
    }
}
