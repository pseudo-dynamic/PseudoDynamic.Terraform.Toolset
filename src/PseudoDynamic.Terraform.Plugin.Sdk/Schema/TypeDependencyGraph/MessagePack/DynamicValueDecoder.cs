using MessagePack;
using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.MessagePack
{
    internal sealed class DynamicValueDecoder
    {
        private IServiceProvider _serviceProvider;

        public DynamicValueDecoder(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        private object ActivateComplex(ComplexDefinition complex, IReadOnlyDictionary<string, object?> attributes)
        {
            var reflectionMetadata = complex.ComplexReflectionMetadata;
            var constructorParametersCount = reflectionMetadata.ConstructorParameters.Count;
            var constructorArguments = new object?[constructorParametersCount];
            var constructorSupportedPropertiesCount = reflectionMetadata.ConstructorSupportedProperties.Count;
            var missingConstructorArgumentIndexes = new HashSet<int>(Enumerable.Range(0, constructorParametersCount));

            for (int i = 0; i < constructorSupportedPropertiesCount; i++) {
                var (constructorSupportedParameter, constructorSupportedProperty) = reflectionMetadata.ConstructorSupportedProperties[i];
                var constructorSupportedParameterPosition = constructorSupportedParameter.Position;
                missingConstructorArgumentIndexes.Remove(constructorSupportedParameterPosition);
                var attributeName = reflectionMetadata.PropertyNameAttributeNameMapping[constructorSupportedProperty.Name];
                constructorArguments[constructorSupportedParameterPosition] = attributes[attributeName];
            }

            foreach (var missingConstructorArgumentIndex in missingConstructorArgumentIndexes) {
                var constructorParameter = reflectionMetadata.ConstructorParameters[missingConstructorArgumentIndex];
                constructorArguments[missingConstructorArgumentIndex] = _serviceProvider.GetRequiredService(constructorParameter.ParameterType);
            }

            return reflectionMetadata.PrimaryConstructor.Invoke(constructorArguments);
        }

        private object DecodeComplex(ComplexDefinition block, ref MessagePackReader reader, DecodingOptions options)
        {
            if (block is not IAbstractAttributeAccessor abstractAttributeAccessor) {
                throw new NotImplementedException($"Block does not implement {typeof(IAbstractAttributeAccessor).FullName}");
            }

            var parsedAttributes = new Dictionary<string, object?>();
            var attributesCount = reader.ReadMapHeader();

            for (int i = 0; i < attributesCount; i++) {
                var attributeName = reader.ReadString();
                var attribute = abstractAttributeAccessor.GetAbstractAttribute(attributeName);
                var attributeRequired = attribute.IsRequired;
                var attributeValueResult = DecodeValue(attribute.Value, ref reader, options, attributeRequired);

                if (!attributeValueResult.IsUnknown && attributeRequired && attributeValueResult.IsNull) {
                    options.Reports.Error($"The attribute \"{attributeName}\" is required and must not be null");
                }

                parsedAttributes.Add(attributeName, attributeValueResult.Value);
            }

            return ActivateComplex(block, parsedAttributes);
        }

        private string DecodeString(ref MessagePackReader reader) => reader.ReadString();

        private object DecodeNonNullValue(ValueDefinition value, ref MessagePackReader reader, DecodingOptions options) => value.TypeConstraint switch {
            TerraformTypeConstraint.String => DecodeString(ref reader),
            TerraformTypeConstraint.Block => DecodeComplex((BlockDefinition)value, ref reader, options),
            _ => throw new NotSupportedException()
        };

        private ValueResult DecodeValue(ValueDefinition value, ref MessagePackReader reader, DecodingOptions options, bool isResultRequired = false)
        {
            object? value2;
            bool isNull;
            bool isUnknown;

            if (reader.TryReadNil()) {
                value2 = null;
                isNull = true;
                isUnknown = false;
            } else if (reader.NextMessagePackType == MessagePackType.Extension) {
                var extension = reader.ReadExtensionFormat();
                value2 = null;
                isNull = true;
                isUnknown = extension.TypeCode == 0;
            } else {
                value2 = DecodeNonNullValue(value, ref reader, options);
                isNull = false;
                isUnknown = false;
            }

            object? value3 = value.IsWrappedByTerraformValue
                ? TerraformValueActivator.CreateInstance(value.SourceType, !isResultRequired, value2, isNull, isUnknown)
                : value2;

            return new ValueResult(value3, isNull, isUnknown);
        }

        /// <summary>
        /// Reads the schema.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="encodedBytes">Message Pack encoded bytes.</param>
        /// <param name="options"></param>
        public object DecodeSchema(BlockDefinition block, ReadOnlyMemory<byte> encodedBytes, DecodingOptions options)
        {
            if (options is null) {
                throw new ArgumentNullException(nameof(options));
            }

            var reader = new MessagePackReader(encodedBytes);
            return DecodeComplex(block, ref reader, options) ?? throw new InvalidOperationException("A non-nil map header was expected");
        }

        public class ValueResult
        {
            public object? Value { get; }
            public bool IsNull { get; }
            public bool IsUnknown { get; }

            public ValueResult(object? result, bool isNull, bool isUnknown)
            {
                Value = result;
                IsNull = isNull;
                IsUnknown = isUnknown;
            }
        }

        internal class DecodingOptions
        {
            public Reports Reports {
                get => _reports ??= new Reports();
                init => _reports = value;
            }

            private Reports? _reports;
        }
    }
}
