using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    /// <summary>
    /// Holds properties and primary constructor informations.
    /// </summary>
    internal class ComplexTypeMetadata
    {
        public static ComplexTypeMetadata FromVisitContext(VisitContext context)
        {
            if (context.ContextType != VisitContextType.Complex) {
                throw new ArgumentException($"Expected context of type {VisitContextType.Complex.Id} but it was {context.ContextType}");
            }

            var visitType = context.VisitType;

            var allConstructors = visitType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo constructor;

            if (allConstructors.Length == 0) {
                throw new ComplexTypeException($"Expected at least one public constructor in type of {visitType.FullName}");
            } else if (allConstructors.Length == 1) {
                constructor = allConstructors.Single();
            } else {
                constructor = allConstructors.Single(x => x.GetCustomAttribute<BlockConstructorAttribute>(inherit: false) != null);
            }

            var allReadableProperties = visitType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetGetMethod(nonPublic: false) != null
                            && x.GetCustomAttribute<AttributeIgnoreAttribute>(inherit: false) == null)
                .ToArray();

            var constructorParameters = constructor.GetParameters()
                .ToDictionary(x => x.Name!, StringComparer.InvariantCultureIgnoreCase);

            var constructorSupportedProperties = allReadableProperties
                .Where(x => constructorParameters.ContainsKey(x.Name))
                .ToArray();

            var nonConstructorSupportedProperties = allReadableProperties
                .Except(constructorSupportedProperties)
                .Where(x => x.GetSetMethod(nonPublic: false) != null)
                .ToArray();

            return new ComplexTypeMetadata(
                constructor,
                constructorParameters,
                constructorSupportedProperties,
                nonConstructorSupportedProperties);
        }

        public ConstructorInfo Constructor { get; }
        public Dictionary<string, ParameterInfo> ConstructorParameters { get; }
        public IEnumerable<PropertyInfo> AllProperties => NonConstructorSupportedProperties.Concat(ConstructorSupportedProperties);
        public PropertyInfo[] ConstructorSupportedProperties { get; }
        public PropertyInfo[] NonConstructorSupportedProperties { get; }

        public ComplexTypeMetadata(
            ConstructorInfo constructor,
            Dictionary<string, ParameterInfo> constructorParameters,
            PropertyInfo[] constructorSupportedProperties,
            PropertyInfo[] nonConstructorSupportedProperties)
        {
            Constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            ConstructorParameters = constructorParameters ?? throw new ArgumentNullException(nameof(constructorParameters));
            ConstructorSupportedProperties = constructorSupportedProperties ?? throw new ArgumentNullException(nameof(constructorSupportedProperties));
            NonConstructorSupportedProperties = nonConstructorSupportedProperties ?? throw new ArgumentNullException(nameof(nonConstructorSupportedProperties));
        }
    }
}
