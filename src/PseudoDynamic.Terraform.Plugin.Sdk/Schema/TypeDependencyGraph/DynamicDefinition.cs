using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal sealed record DynamicDefinition : ValueDefinition
    {
        internal static DynamicDefinition Uncomputed() => new(UncomputedSourceType);

        public override TerraformTypeConstraint TypeConstraint => TerraformTypeConstraint.Dynamic;
        public override TerraformDefinitionType DefinitionType => TerraformDefinitionType.Dynamic;
        public BlockNode<IVisitPropertySegmentContext> DynamicNode { get; }

        private DynamicDefinition(Type sourceType) : base(sourceType) =>
            DynamicNode = null!;

        public DynamicDefinition(BlockNode<IVisitPropertySegmentContext> dynamicNode) : base(typeof(object)) =>
            DynamicNode = dynamicNode ?? throw new ArgumentNullException(nameof(dynamicNode));

        protected internal override void Visit(TerraformDefinitionVisitor visitor) => visitor.VisitDynamic(this);

        public bool Equals(DynamicDefinition? other) => PreventRCS1036(base.Equals(other));

        public override int GetHashCode() => PreventRCS1036(base.GetHashCode());
    }
}
