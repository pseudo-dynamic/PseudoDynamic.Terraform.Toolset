using System.Numerics;
using System.Text;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.MessagePack
{
    internal sealed class DynamicValueDecoder
    {
        private static GenericTypeAccessor ListAccessor = new GenericTypeAccessor(typeof(List<>));
        private static GenericTypeAccessor SetAccessor = new GenericTypeAccessor(typeof(HashSet<>));
        private static GenericTypeAccessor DictionaryAccessor = new GenericTypeAccessor(typeof(Dictionary<,>));

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

        private object DecodeComplex(ComplexDefinition complex, ref MessagePackReader reader, DecodingOptions options)
        {
            if (complex is not IAbstractAttributeAccessor abstractAttributeAccessor) {
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

            return ActivateComplex(complex, parsedAttributes);
        }

        private string DecodeString(ref MessagePackReader reader) => reader.ReadString();

        private object DecodeNumber(ValueDefinition value, ref MessagePackReader reader)
        {
            // We make it dynamic, so the explicit BigInteger user-conversion works.
            dynamic? number = TryReadNumber(ref reader);

            if (number != null) {
                if (value.SourceType == typeof(BigInteger)) {
                    return (BigInteger)number;
                } else {
                    return Convert.ChangeType(number, value.SourceType);
                }
            }

            var numberUtf8 = TryReadUtf8(ref reader);

            if (numberUtf8 != null) {
                if (value.SourceType == typeof(BigInteger)) {
                    return BigInteger.Parse(numberUtf8);
                } else {
                    return Convert.ChangeType(numberUtf8, value.SourceType);
                }
            }

            throw new DynamicValueDecodingException($"The Terraform number \"{numberUtf8}\" is not representable as {value.SourceType.FullName}");

            object? TryReadNumber(ref MessagePackReader reader) => reader.NextCode switch {
                MessagePackCode.UInt8 => reader.ReadByte(),
                MessagePackCode.Int8 => reader.ReadSByte(),
                MessagePackCode.UInt16 => reader.ReadUInt16(),
                MessagePackCode.Int16 => reader.ReadInt16(),
                MessagePackCode.UInt32 => reader.ReadUInt32(),
                MessagePackCode.Int32 => reader.ReadInt32(),
                MessagePackCode.Float32 => reader.ReadSingle(),
                MessagePackCode.UInt64 => reader.ReadUInt64(),
                MessagePackCode.Int64 => reader.ReadInt64(),
                MessagePackCode.Float64 => reader.ReadDouble(),
                _ => null
            };

            static string? TryReadUtf8(ref MessagePackReader reader)
            {
                var bytes = reader.ReadStringSequence();

                return bytes.HasValue
                    ? Encoding.UTF8.GetString(bytes.Value)
                    : null;
            }
        }

        private bool DecodeBool(ref MessagePackReader reader) => reader.ReadBoolean();

        private object DecodeList(MonoRangeDefinition list, ref MessagePackReader reader, DecodingOptions options)
        {
            var itemCount = reader.ReadArrayHeader();

            var listAccessor = ListAccessor.GetTypeAccessor(list.Item.WrappedSourceType);
            dynamic items = listAccessor.InvokeConstructor(static x => x.GetPublicInstanceConstructor);
            var addItem = listAccessor.GetMethod(nameof(IList<object>.Add));

            for (int i = 0; i < itemCount; i++) {
                var item = DecodeValue(list.Item, ref reader, options).Value;
                addItem.Invoke(items, new[] { item });
            }

            return items;
        }

        private object DecodeSet(MonoRangeDefinition set, ref MessagePackReader reader, DecodingOptions options)
        {
            var itemCount = reader.ReadArrayHeader();

            var setAccessor = SetAccessor.GetTypeAccessor(set.Item.WrappedSourceType);
            dynamic items = setAccessor.InvokeConstructor(static x => x.GetPublicInstanceConstructor);
            var addItem = setAccessor.GetMethod(nameof(ISet<object>.Add));

            for (int i = 0; i < itemCount; i++) {
                var item = DecodeValue(set.Item, ref reader, options).Value;
                addItem.Invoke(items, new[] { item });
            }

            return items;
        }

        private object DecodeMap(MapDefinition map, ref MessagePackReader reader, DecodingOptions options)
        {
            var itemCount = reader.ReadMapHeader();

            var mapAccessor = DictionaryAccessor.GetTypeAccessor(map.Key.WrappedSourceType, map.Value.WrappedSourceType);
            dynamic items = mapAccessor.InvokeConstructor(static x => x.GetPublicInstanceConstructor);
            var addItem = mapAccessor.GetMethod(nameof(IDictionary<object, object>.Add));

            for (int i = 0; i < itemCount; i++) {
                var key = reader.ReadString();
                var value = DecodeValue(map.Value, ref reader, options).Value;
                addItem.Invoke(items, new[] { key, value });
            }

            return items;
        }

        private object DecodeNonNullValue(ValueDefinition value, ref MessagePackReader reader, DecodingOptions options) => value.TypeConstraint switch {
            TerraformTypeConstraint.String => DecodeString(ref reader),
            TerraformTypeConstraint.Number => DecodeNumber(value, ref reader),
            TerraformTypeConstraint.Bool => DecodeBool(ref reader),
            TerraformTypeConstraint.List => DecodeList((MonoRangeDefinition)value, ref reader, options),
            TerraformTypeConstraint.Set => DecodeSet((MonoRangeDefinition)value, ref reader, options),
            TerraformTypeConstraint.Map => DecodeMap((MapDefinition)value, ref reader, options),
            TerraformTypeConstraint.Object => DecodeComplex((ObjectDefinition)value, ref reader, options),
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
                ? TerraformValue.CreateInstance(value.SourceType, !isResultRequired, value2, isNull, isUnknown)
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
