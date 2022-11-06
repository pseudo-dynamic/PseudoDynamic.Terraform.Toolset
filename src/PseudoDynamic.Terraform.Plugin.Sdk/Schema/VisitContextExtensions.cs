using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal static class VisitContextExtensions
    {
        internal static TerraformTypeConstraint DetermineExplicitTypeConstraint<TContext>(this TContext context)
            where TContext : IVisitPropertySegmentContext
        {
            var property = context.Property;
            var valueType = context.VisitType;

            var implicitTypeConstraints = context.ImplicitTypeConstraints;

            if (implicitTypeConstraints.Contains(TerraformTypeConstraint.Dynamic)) {
                return TerraformTypeConstraint.Dynamic;
            }

            var valueAttribute = property.GetCustomAttribute<ValueAttribute>(inherit: true);
            TerraformTypeConstraint typeConstraintResult;

            if (valueAttribute is not null) {
                var explicitTypeConstraint = valueAttribute.TypeConstraint;
                var isExplicitComplex = explicitTypeConstraint.IsComplex();

                if (isExplicitComplex || implicitTypeConstraints.Contains(explicitTypeConstraint)) {
                    typeConstraintResult = explicitTypeConstraint;
                } else {
                    throw new InvalidOperationException($"The \"{typeof(ValueAttribute).FullName}\" attribute on \"{context.Property.GetPath()}\" property wants to be "
                        + $"a \"{explicitTypeConstraint}\" Terraform type constraint but the \"{property.PropertyType.Name}\" property type does not support it");
                }
            } else if (implicitTypeConstraints.Count == 0) {
                throw new InvalidOperationException($"The \"{context.Property.GetPath()}\" property does not implement at least one Terraform type constraint");
            } else if (implicitTypeConstraints.Count > 1) {
                throw new InvalidOperationException($"The \"{context.Property.GetPath()}\" property cannot implement more than one Terraform type constraints. " +
                    "The following Terraform type constraints have been evaluated: "
                    + string.Join(", ", implicitTypeConstraints.Select(x => x.ToString())));
            } else {
                typeConstraintResult = implicitTypeConstraints.Single();
            }

            return typeConstraintResult;
        }
    }
}
