using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal static class BlockNodeExtensions
    {
        public static bool TryUnwrapProperty(this BlockNode node, out BlockNode<IVisitPropertySegmentContext> unwrappedNode)
        {
            if (node.Context.ContextType == VisitContextType.Property && node.Nodes.Count != 0) {
                unwrappedNode = node.Single().AsContext<IVisitPropertySegmentContext>();
                return true;
            }

            unwrappedNode = node.AsContext<IVisitPropertySegmentContext>();
            return false;
        }

        public static bool TryUnwrapTerraformValue(this BlockNode node, out BlockNode<IVisitPropertySegmentContext> unwrappedNode)
        {
            if (node.SingleOrDefault(childNode => childNode.Context.ContextType == TerraformVisitContextType.TerraformValue) is BlockNode childNode) {
                unwrappedNode = childNode.AsContext<IVisitPropertySegmentContext>();
                unwrappedNode.EnsureNodesNotHavingNestedTerraformValue();
                return true;
            }

            unwrappedNode = node.AsContext<IVisitPropertySegmentContext>();
            return false;
        }

        public static void EnsureNodesNotHavingNestedTerraformValue(this BlockNode node)
        {
            if (node.Any(x => x.Context.ContextType == TerraformVisitContextType.TerraformValue)) {
                var errorMessage = node.Context.ContextType == TerraformVisitContextType.TerraformValue
                    ? $"In the dependency graph of {node.Context.VisitedType.FullName}, another nested terraform value is not allowed"
                    : $"In the dependency graph of {node.Context.VisitedType.FullName}, a nested terraform value is not allowed";

                throw new NestedTerraformValueException(errorMessage);
            }
        }

        //// TODO: ......................
        //internal static TerraformTypeConstraint DetermineTypeConstraintForBlock(this BlockNode<IVisitPropertySegmentContext> node, IReadOnlySet<TerraformTypeConstraint> implicitTypeConstraints)
        //{
        //    var property = node.Context.Property;
        //    var valueType = node.Context.VisitedType;
        //    var valueAttribute = property.GetCustomAttribute<ValueAttribute>(inherit: true);

        //    var implicitTypeConstraints = TerraformTypeConstraintEvaluator.Default.Evaluate(valueType);
        //    TerraformTypeConstraint typeConstraintResult;
        //    TerraformTypeConstraint? singleImplicitTypeConstraint = null;

        //    if (valueAttribute is not null) {
        //        var explicitTerraformType = valueAttribute.TypeConstraint;
        //        var isExplicitTerraformBlockType = explicitTerraformType.IsBlockType();

        //        if (isExplicitTerraformBlockType && implicitTypeConstraints.Contains(TerraformTypeConstraint.Object)
        //            || implicitTypeConstraints.Contains(explicitTerraformType)) {
        //            typeConstraintResult = explicitTerraformType;
        //        } else if (isExplicitTerraformBlockType && implicitTypeConstraints.Count == 1
        //              && GetSingleImplicitTypeConstraint().IsRangeType()) {
        //            typeConstraintResult = explicitTerraformType;
        //        } else {
        //            throw new InvalidOperationException($"The \"{typeof(ValueAttribute).FullName}\" attribute on \"{property.DeclaringType?.FullName}.{property.Name}\" property wants to be "
        //                + $"a \"{explicitTerraformType}\" Terraform type constraint but the \"{property.PropertyType.Name}\" property type does not support it");
        //        }
        //    } else {
        //        typeConstraintResult = GetSingleImplicitTypeConstraint();
        //    }

        //    return typeConstraintResult;

        //    TerraformTypeConstraint GetSingleImplicitTypeConstraint()
        //    {
        //        if (singleImplicitTypeConstraint.HasValue) {
        //            return singleImplicitTypeConstraint.Value;
        //        }

        //        if (implicitTypeConstraints.Count == 0) {
        //            throw new InvalidOperationException($"The \"{property.Name}\" property does not implement at least one Terraform type constraint");
        //        } else if (implicitTypeConstraints.Count > 1) {
        //            throw new InvalidOperationException($"The \"{property.Name}\" property cannot implement more than one Terraform type constraints. " +
        //                "The following Terraform type constraints have been evaluated: "
        //                + string.Join(", ", implicitTypeConstraints.Select(x => x.ToString())));
        //        }

        //        singleImplicitTypeConstraint = implicitTypeConstraints.Single();
        //        return singleImplicitTypeConstraint.Value;
        //    }
        //}
    }
}
