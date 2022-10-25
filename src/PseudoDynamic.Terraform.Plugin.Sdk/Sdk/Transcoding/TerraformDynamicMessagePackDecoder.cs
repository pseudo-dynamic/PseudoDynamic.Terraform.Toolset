using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    internal sealed class TerraformDynamicMessagePackDecoder
    {
        private readonly static GenericTypeAccessor ListAccessor = new GenericTypeAccessor(typeof(List<>));
        private readonly static GenericTypeAccessor SetAccessor = new GenericTypeAccessor(typeof(HashSet<>));
        private readonly static GenericTypeAccessor DictionaryAccessor = new GenericTypeAccessor(typeof(Dictionary<,>));

        private IServiceProvider _serviceProvider;

        public TerraformDynamicMessagePackDecoder(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        private string DecodeString(ref MessagePackReader reader) => reader.ReadString();

        private object DecodeNumber(ref MessagePackReader reader, ValueDefinition value)
        {
            if (TryReadUtf8(ref reader, out var utf8Number)) {
                if (value.SourceType == typeof(BigInteger)) {
                    return BigInteger.Parse(utf8Number);
                } else {
                    return Convert.ChangeType(utf8Number, value.SourceType);
                }
            }

            // We make it dynamic, so the explicit BigInteger user-conversion works.
            dynamic? number = TryReadMessagePackDrivenNumber(ref reader)
                ?? TryReadSchemaDrivenNumber(ref reader, value);

            if (number != null) {
                if (value.SourceType == typeof(BigInteger)) {
                    return (BigInteger)number;
                } else {
                    return Convert.ChangeType(number, value.SourceType);
                }
            }

            throw new TerraformDynamicMessagePackDecodingException($"The Terraform number cannot be converted to {value.SourceType.FullName}");

            static object? TryReadMessagePackDrivenNumber(ref MessagePackReader reader) => reader.NextCode switch {
                MessagePackCode.UInt8 => reader.ReadByte(),
                MessagePackCode.Int8 => reader.ReadSByte(),
                MessagePackCode.UInt16 => reader.ReadUInt16(),
                MessagePackCode.Int16 => reader.ReadInt16(),
                MessagePackCode.UInt32 => reader.ReadUInt32(),
                MessagePackCode.Int32 => reader.ReadInt32(),
                MessagePackCode.UInt64 => reader.ReadUInt64(),
                MessagePackCode.Int64 => reader.ReadInt64(),
                MessagePackCode.Float32 => reader.ReadSingle(),
                MessagePackCode.Float64 => reader.ReadDouble(),
                _ => null
            };

            static object? TryReadSchemaDrivenNumber(ref MessagePackReader reader, ValueDefinition value) => Type.GetTypeCode(value.SourceType) switch {
                TypeCode.Byte => reader.ReadByte(),
                TypeCode.SByte => reader.ReadSByte(),
                TypeCode.UInt16 => reader.ReadUInt16(),
                TypeCode.Int16 => reader.ReadInt16(),
                TypeCode.UInt32 => reader.ReadUInt32(),
                TypeCode.Int32 => reader.ReadInt32(),
                TypeCode.UInt64 => reader.ReadUInt64(),
                TypeCode.Int64 => reader.ReadInt64(),
                TypeCode.Single => reader.ReadSingle(),
                TypeCode.Double => reader.ReadDouble(),
                _ => null
            };

            static bool TryReadUtf8(ref MessagePackReader reader, [NotNullWhen(true)] out string? utf8String)
            {
                if (reader.NextMessagePackType == MessagePackType.String) {
                    var bytes = reader.ReadStringSequence();

                    utf8String = bytes.HasValue
                        ? Encoding.UTF8.GetString(bytes.Value)
                        : string.Empty;

                    return true;
                }

                utf8String = null;
                return false;
            }
        }

        private bool DecodeBool(ref MessagePackReader reader) => reader.ReadBoolean();

        private object DecodeList(ref MessagePackReader reader, MonoRangeDefinition list, DecodingOptions options)
        {
            var itemCount = reader.ReadArrayHeader();

            var listAccessor = ListAccessor.MakeGenericTypeAccessor(list.Item.OuterType);
            var items = listAccessor.CreateInstance(static x => x.GetPublicInstanceActivator);
            var addItem = listAccessor.GetMethodCaller(nameof(IList<object>.Add));

            for (int i = 0; i < itemCount; i++) {
                var item = DecodeValue(ref reader, list.Item, options).Value;
                addItem.Invoke(items, new[] { item });
            }

            return items;
        }

        private object DecodeSet(ref MessagePackReader reader, MonoRangeDefinition set, DecodingOptions options)
        {
            var itemCount = reader.ReadArrayHeader();

            var setAccessor = SetAccessor.MakeGenericTypeAccessor(set.Item.OuterType);
            var items = setAccessor.CreateInstance(static x => x.GetPublicInstanceActivator);
            var addItem = setAccessor.GetMethodCaller(nameof(ISet<object>.Add));

            for (int i = 0; i < itemCount; i++) {
                var item = DecodeValue(ref reader, set.Item, options).Value;
                addItem.Invoke(items, new[] { item });
            }

            return items;
        }

        private object DecodeMap(ref MessagePackReader reader, MapDefinition map, DecodingOptions options)
        {
            var itemCount = reader.ReadMapHeader();

            var mapAccessor = DictionaryAccessor.MakeGenericTypeAccessor(map.Key.OuterType, map.Value.OuterType);
            var items = mapAccessor.CreateInstance(static x => x.GetPublicInstanceActivator);
            var addItem = mapAccessor.GetMethodCaller(nameof(IDictionary<object, object>.Add));

            for (int i = 0; i < itemCount; i++) {
                var key = reader.ReadString();
                var value = DecodeValue(ref reader, map.Value, options).Value;
                addItem.Invoke(items, new[] { key, value });
            }

            return items;
        }

        /// <summary>
        /// Enables deserialization through constructor, where constructor is used to inject just decoded block
        /// attributes as parameters. Other constructor parameters are resolved by service provider.
        /// </summary>
        /// <param name="complex"></param>
        /// <param name="attributes"></param>
        /// <returns>The instantiated instance.</returns>
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

        private object DecodeComplex(ref MessagePackReader reader, ComplexDefinition complex, DecodingOptions options)
        {
            if (complex is not IAttributeAccessor abstractAttributeAccessor) {
                throw new NotImplementedException($"Complex definition does not implement {typeof(IAttributeAccessor).FullName}");
            }

            var parsedAttributes = new Dictionary<string, object?>();
            var attributesCount = reader.ReadMapHeader();

            for (int i = 0; i < attributesCount; i++) {
                var attributeName = reader.ReadString();
                var attribute = abstractAttributeAccessor.GetAttribute(attributeName);
                var attributeRequired = attribute.IsRequired;
                var attributeValueResult = DecodeValue(ref reader, attribute.Value, options, attributeRequired);

                if (!attributeValueResult.IsUnknown && attributeRequired && attributeValueResult.IsNull) {
                    options.Reports.Error($"The attribute \"{attributeName}\" is required and must not be null");
                }

                parsedAttributes.Add(attributeName, attributeValueResult.Value);
            }

            return ActivateComplex(complex, parsedAttributes);
        }

        private object DecodeNonNullValue(ValueDefinition value, ref MessagePackReader reader, DecodingOptions options) => value.TypeConstraint switch {
            TerraformTypeConstraint.String => DecodeString(ref reader),
            TerraformTypeConstraint.Number => DecodeNumber(ref reader, value),
            TerraformTypeConstraint.Bool => DecodeBool(ref reader),
            TerraformTypeConstraint.List => DecodeList(ref reader, (MonoRangeDefinition)value, options),
            TerraformTypeConstraint.Set => DecodeSet(ref reader, (MonoRangeDefinition)value, options),
            TerraformTypeConstraint.Map => DecodeMap(ref reader, (MapDefinition)value, options),
            TerraformTypeConstraint.Object => DecodeComplex(ref reader, (ObjectDefinition)value, options),
            TerraformTypeConstraint.Block => DecodeComplex(ref reader, (BlockDefinition)value, options),
            _ => throw new NotImplementedException($"The decoding of this Terraform constraint type is not implemented: {value.TypeConstraint}")
        };

        private ValueResult DecodeValue(ref MessagePackReader reader, ValueDefinition value, DecodingOptions options, bool isResultRequired = false)
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
        /// Decodes the schema.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="block"></param>
        /// <param name="options"></param>
        public object DecodeSchema(ref MessagePackReader reader, BlockDefinition block, DecodingOptions options)
        {
            if (options is null) {
                throw new ArgumentNullException(nameof(options));
            }

            return DecodeComplex(ref reader, block, options) ?? throw new InvalidOperationException("The very first decoding block must have a non-nil map header");
        }

        public object DecodeSchema(ReadOnlyMemory<byte> memory, BlockDefinition block, DecodingOptions options)
        {
            var reader = new MessagePackReader(memory);
            return DecodeSchema(ref reader, block, options);
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
