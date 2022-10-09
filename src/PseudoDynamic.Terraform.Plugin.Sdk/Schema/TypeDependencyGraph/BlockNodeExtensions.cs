using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

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

        internal static TerraformTypeConstraint DetermineTypeConstraint(this BlockNode<IVisitPropertySegmentContext> node)
        {
            var property = node.Context.Property;
            var valueType = node.Context.VisitedType;
            var terraformTypeAttribute = property.GetCustomAttribute<ValueAttribute>(inherit: true);
            var hasTerraformTypeAttribute = terraformTypeAttribute is not null;

            var guessedTerraformTypes = TerraformTypeConstraintEvaluator.Default.Evaluate(valueType);
            TerraformTypeConstraint terraformValueType;

            if (hasTerraformTypeAttribute) {
                var explicitTerraformType = terraformTypeAttribute!.TypeConstraint;
                var isExplicitTerraformBlockType = explicitTerraformType.IsBlockType();

                if (isExplicitTerraformBlockType && guessedTerraformTypes.Contains(TerraformTypeConstraint.Object)
                    || guessedTerraformTypes.Contains(explicitTerraformType)) {
                    terraformValueType = explicitTerraformType;
                } else if (isExplicitTerraformBlockType && guessedTerraformTypes.Count == 1
                      && guessedTerraformTypes.Single().IsRangeType()) {
                    terraformValueType = explicitTerraformType;
                } else {
                    throw new InvalidOperationException($"The \"{typeof(ValueAttribute).FullName}\" attribute on \"{property.DeclaringType?.FullName}.{property.Name}\" property wants to be "
                        + $"a \"{explicitTerraformType}\" Terraform value type but the \"{property.PropertyType.Name}\" property type does not support it");
                }
            } else if (guessedTerraformTypes.Count == 0) {
                throw new InvalidOperationException($"The \"{property.Name}\" property does implement at least Terraform value type");
            } else if (guessedTerraformTypes.Count > 1) {
                throw new InvalidOperationException($"The \"{property.Name}\" property cannot implement more than one Terraform value type. " +
                    "The following Terraform value types have been found: "
                    + string.Join(", ", guessedTerraformTypes.Select(x => x.ToString())));
            } else {
                terraformValueType = guessedTerraformTypes.Single();
            }

            return terraformValueType;
        }
    }
}
