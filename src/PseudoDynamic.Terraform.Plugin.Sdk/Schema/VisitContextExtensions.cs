using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal static class VisitContextExtensions
    {
        internal static TerraformTypeConstraint DetermineExplicitTypeConstraint<TContext>(this TContext context, out IReadOnlySet<TerraformTypeConstraint> implicitTypeConstraints)
            where TContext : IVisitPropertySegmentContext
        {
            var property = context.Property;
            var valueType = context.VisitedType;
            var valueAttribute = property.GetCustomAttribute<ValueAttribute>(inherit: true);

            implicitTypeConstraints = TerraformTypeConstraintEvaluator.Default.Evaluate(valueType);
            TerraformTypeConstraint typeConstraintResult;

            if (valueAttribute is not null) {
                var explicitTypeConstraint = valueAttribute.TypeConstraint;
                var isExplicitBlockLike = explicitTypeConstraint.IsBlockLike();

                if (isExplicitBlockLike || implicitTypeConstraints.Contains(explicitTypeConstraint)) {
                    typeConstraintResult = explicitTypeConstraint;
                } else {
                    throw new InvalidOperationException($"The \"{typeof(ValueAttribute).FullName}\" attribute on \"{property.DeclaringType?.FullName}.{property.Name}\" property wants to be "
                        + $"a \"{explicitTypeConstraint}\" Terraform type constraint but the \"{property.PropertyType.Name}\" property type does not support it");
                }
            } else if (implicitTypeConstraints.Count == 0) {
                throw new InvalidOperationException($"The \"{property.Name}\" property does not implement at least one Terraform type constraint");
            } else if (implicitTypeConstraints.Count > 1) {
                throw new InvalidOperationException($"The \"{property.Name}\" property cannot implement more than one Terraform type constraints. " +
                    "The following Terraform type constraints have been evaluated: "
                    + string.Join(", ", implicitTypeConstraints.Select(x => x.ToString())));
            } else {
                typeConstraintResult = implicitTypeConstraints.Single();
            }

            return typeConstraintResult;
        }
    }
}
