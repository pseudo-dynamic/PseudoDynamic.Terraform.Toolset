using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal interface IVisitPropertySegmentContext : IVisitContext
    {
        PropertyInfo Property { get; }

        /// <summary>
        /// Consists of the full name of <see cref="MemberInfo.DeclaringType"/> and the property name of <see cref="Property"/>.
        /// </summary>
        string PropertyPath => $"{Property.DeclaringType!.FullName}.{Property.Name}";

        NullabilityInfo NullabilityInfo { get; }

        /// <summary>
        /// The property segment depth starting with zero.
        /// </summary>
        int SegmentDepth { get; }
    }
}
