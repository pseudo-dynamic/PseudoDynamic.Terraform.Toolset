using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    /// <summary>
    /// Holds properties and primary constructor informations.
    /// </summary>
    internal record class ComplexTypeMetadata
    {
        public static ComplexTypeMetadata FromVisitContext(VisitContext context)
        {
            if (context.ContextType != VisitContextType.Complex) {
                throw new ArgumentException($"Expected context of type {VisitContextType.Complex.Id} but it was {context.ContextType}");
            }

            var visitType = context.VisitType;

            var allConstructors = visitType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo primaryConstructor;

            if (allConstructors.Length == 0) {
                throw new ComplexTypeException($"Expected at least one public constructor in type of {visitType.FullName}");
            } else if (allConstructors.Length == 1) {
                primaryConstructor = allConstructors.Single();
            } else {
                primaryConstructor = allConstructors.Single(x => x.GetCustomAttribute<BlockConstructorAttribute>(inherit: false) != null);
            }

            var allReadableProperties = visitType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetGetMethod(nonPublic: false) != null
                            && x.GetCustomAttribute<AttributeIgnoreAttribute>(inherit: false) == null)
                .ToArray();

            var constructorParameters = primaryConstructor.GetParameters();

            var indexedConstructorParameters = constructorParameters
                .ToDictionary(x => x.Name!, StringComparer.InvariantCultureIgnoreCase);

            var constructorSupportedProperties = allReadableProperties
                .Where(x => indexedConstructorParameters.ContainsKey(x.Name))
                .Select(x => (Parameter: indexedConstructorParameters[x.Name], Property: x))
                .ToList();

            var nonConstructorSupportedProperties = allReadableProperties
                .Except(constructorSupportedProperties.Select(x => x.Property))
                .Where(x => x.GetSetMethod(nonPublic: false) != null)
                .ToList();

            var constructorSupportedPropertiesCount = constructorSupportedProperties.Count;
            var supportedProperties = new PropertyInfo[constructorSupportedPropertiesCount + nonConstructorSupportedProperties.Count];

            for (var i = 0; i < constructorSupportedPropertiesCount; i++) {
                supportedProperties[i] = constructorSupportedProperties[i].Property;
            }

            nonConstructorSupportedProperties.CopyTo(supportedProperties, constructorSupportedPropertiesCount);

            return new ComplexTypeMetadata(
                supportedProperties,
                primaryConstructor,
                constructorParameters,
                constructorSupportedProperties,
                nonConstructorSupportedProperties);
        }

        public IReadOnlyList<PropertyInfo> SupportedProperties { get; }
        public ConstructorInfo PrimaryConstructor { get; }
        public IReadOnlyList<ParameterInfo> ConstructorParameters { get; }
        public IReadOnlyList<(ParameterInfo Parameter, PropertyInfo Property)> ConstructorSupportedProperties { get; }
        public IReadOnlyList<PropertyInfo> NonConstructorSupportedProperties { get; }

        public ComplexTypeMetadata(
            IReadOnlyList<PropertyInfo> supportedProperties,
            ConstructorInfo primaryConstructor,
            IReadOnlyList<ParameterInfo> constructorParameters,
            IReadOnlyList<(ParameterInfo Parameter, PropertyInfo Property)> constructorSupportedProperties,
            IReadOnlyList<PropertyInfo> nonConstructorSupportedProperties)
        {
            SupportedProperties = supportedProperties ?? throw new ArgumentNullException(nameof(supportedProperties));
            PrimaryConstructor = primaryConstructor ?? throw new ArgumentNullException(nameof(primaryConstructor));
            ConstructorParameters = constructorParameters ?? throw new ArgumentNullException(nameof(constructorParameters));
            ConstructorSupportedProperties = constructorSupportedProperties ?? throw new ArgumentNullException(nameof(constructorSupportedProperties));
            NonConstructorSupportedProperties = nonConstructorSupportedProperties ?? throw new ArgumentNullException(nameof(nonConstructorSupportedProperties));
        }
    }
}
