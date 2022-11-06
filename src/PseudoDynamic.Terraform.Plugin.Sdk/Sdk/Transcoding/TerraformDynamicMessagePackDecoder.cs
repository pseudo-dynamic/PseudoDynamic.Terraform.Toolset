using System.Numerics;
using System.Text;
using DotNext.Buffers;
using FastMember;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    internal sealed class TerraformDynamicMessagePackDecoder
    {
        private readonly static GenericTypeAccessor ListAccessor = new(typeof(List<>));
        private readonly static GenericTypeAccessor SetAccessor = new(typeof(HashSet<>));
        private readonly static GenericTypeAccessor DictionaryAccessor = new(typeof(Dictionary<,>));

        private readonly IServiceProvider _serviceProvider;
        private readonly SchemaBuilder _schemaBuilder;

        public TerraformDynamicMessagePackDecoder(IServiceProvider serviceProvider, SchemaBuilder resolver)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _schemaBuilder = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        private TerraformDynamic DecodeDynamic(ref MessagePackReader reader, DynamicDefinition definition)
        {
            System.Buffers.ReadOnlySequence<byte> raw = reader.ReadRaw();
            byte[] bytes = new byte[raw.Length];
            raw.CopyTo(bytes, out _);
            return new TerraformDynamic(definition, bytes, TerraformDynamicEncoding.MessagePack);
        }

        private string DecodeString(ref MessagePackReader reader) => reader.ReadString();

        private object DecodeNumber(ref MessagePackReader reader, ValueDefinition definition)
        {
            if (TryReadUtf8(ref reader, out string? utf8Number)) {
                if (definition.SourceType == typeof(BigInteger)) {
                    return BigInteger.Parse(utf8Number);
                } else {
                    return Convert.ChangeType(utf8Number, definition.SourceType);
                }
            }

            // We make it dynamic, so the explicit BigInteger user-conversion works.
            dynamic? number = TryReadMessagePackDrivenNumber(ref reader)
                ?? TryReadSchemaDrivenNumber(ref reader, definition);

            if (number != null) {
                if (definition.SourceType == typeof(BigInteger)) {
                    return (BigInteger)number;
                } else {
                    return Convert.ChangeType(number, definition.SourceType);
                }
            }

            throw new TerraformDynamicMessagePackDecodingException($"The Terraform number cannot be converted to {definition.SourceType.FullName}");

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

            static object? TryReadSchemaDrivenNumber(ref MessagePackReader reader, ValueDefinition definition) => Type.GetTypeCode(definition.SourceType) switch {
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
                    System.Buffers.ReadOnlySequence<byte>? bytes = reader.ReadStringSequence();

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

        private object DecodeList(ref MessagePackReader reader, MonoRangeDefinition definition, DecodingOptions options)
        {
            int itemCount = reader.ReadArrayHeader();

            Reflection.TypeAccessor listAccessor = ListAccessor.MakeGenericTypeAccessor(definition.Item.OuterType);
            object items = listAccessor.CreateInstance(static x => x.GetPublicInstanceActivator);
            Concurrent.FastReflection.NetStandard.MethodCaller<object, object> addItem = listAccessor.GetMethodCaller(nameof(IList<object>.Add));

            for (int i = 0; i < itemCount; i++) {
                object? item = DecodeValueWithContext(ref reader, definition.Item, options).Value;
                addItem.Invoke(items, new[] { item });
            }

            return items;
        }

        private object DecodeSet(ref MessagePackReader reader, MonoRangeDefinition definition, DecodingOptions options)
        {
            int itemCount = reader.ReadArrayHeader();

            Reflection.TypeAccessor setAccessor = SetAccessor.MakeGenericTypeAccessor(definition.Item.OuterType);
            object items = setAccessor.CreateInstance(static x => x.GetPublicInstanceActivator);
            Concurrent.FastReflection.NetStandard.MethodCaller<object, object> addItem = setAccessor.GetMethodCaller(nameof(ISet<object>.Add));

            for (int i = 0; i < itemCount; i++) {
                object? item = DecodeValueWithContext(ref reader, definition.Item, options).Value;
                addItem.Invoke(items, new[] { item });
            }

            return items;
        }

        private object DecodeMap(ref MessagePackReader reader, MapDefinition definition, DecodingOptions options)
        {
            int itemCount = reader.ReadMapHeader();

            Reflection.TypeAccessor mapAccessor = DictionaryAccessor.MakeGenericTypeAccessor(definition.Key.OuterType, definition.Value.OuterType);
            object items = mapAccessor.CreateInstance(static x => x.GetPublicInstanceActivator);
            Concurrent.FastReflection.NetStandard.MethodCaller<object, object> addItem = mapAccessor.GetMethodCaller(nameof(IDictionary<object, object>.Add));

            for (int i = 0; i < itemCount; i++) {
                // CONSIDER: allow key type other than string
                string key = reader.ReadString();
                object? value = DecodeValueWithContext(ref reader, definition.Value, options).Value;
                addItem.Invoke(items, new[] { key, value });
            }

            return items;
        }

        /// <summary>
        /// Enables deserialization through constructor, where constructor is used to inject just decoded block
        /// attributes as parameters. Other constructor parameters are resolved by service provider.
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="attributes"></param>
        /// <returns>The instantiated instance.</returns>
        private object ActivateComplex(ComplexDefinition definition, IReadOnlyDictionary<string, object?> attributes)
        {
            ComplexReflectionMetadata reflectionMetadata = definition.ComplexReflectionMetadata;
            int constructorParametersCount = reflectionMetadata.ConstructorParameters.Count;
            object?[] constructorArguments = new object?[constructorParametersCount];
            int constructorSupportedPropertiesCount = reflectionMetadata.ConstructorSupportedProperties.Count;
            HashSet<int> missingConstructorArgumentIndexes = new(Enumerable.Range(0, constructorParametersCount));

            for (int i = 0; i < constructorSupportedPropertiesCount; i++) {
                (System.Reflection.ParameterInfo constructorSupportedParameter, System.Reflection.PropertyInfo constructorSupportedProperty) = reflectionMetadata.ConstructorSupportedProperties[i];
                int constructorSupportedParameterPosition = constructorSupportedParameter.Position;
                missingConstructorArgumentIndexes.Remove(constructorSupportedParameterPosition);
                string attributeName = reflectionMetadata.PropertyNameAttributeNameMapping[constructorSupportedProperty.Name];
                constructorArguments[constructorSupportedParameterPosition] = attributes[attributeName];
            }

            foreach (int missingConstructorArgumentIndex in missingConstructorArgumentIndexes) {
                System.Reflection.ParameterInfo constructorParameter = reflectionMetadata.ConstructorParameters[missingConstructorArgumentIndex];
                constructorArguments[missingConstructorArgumentIndex] = _serviceProvider.GetRequiredService(constructorParameter.ParameterType);
            }

            object instance = reflectionMetadata.PrimaryConstructor.Invoke(constructorArguments);

            if (reflectionMetadata.NonConstructorSupportedProperties.Count > 0) {
                ObjectAccessor instanceAccessor = ObjectAccessor.Create(instance);

                foreach (System.Reflection.PropertyInfo nonConstructorSupportedProperty in reflectionMetadata.NonConstructorSupportedProperties) {
                    string propertyName = nonConstructorSupportedProperty.Name;
                    string attributeName = reflectionMetadata.PropertyNameAttributeNameMapping[propertyName];
                    object? propertyValue = attributes[attributeName];

                    if (nonConstructorSupportedProperty.PropertyType.IsValueType && propertyValue is null) {
                        continue;
                    }

                    instanceAccessor[propertyName] = propertyValue;
                }
            }

            return instance;
        }

        private object? DecodeNullableComplex(ref MessagePackReader reader, ComplexDefinition definition, DecodingOptions options)
        {
            if (reader.IsNil) {
                return null;
            }

            if (definition is not IAttributeAccessor abstractAttributeAccessor) {
                throw new NotImplementedException($"Complex definition does not implement {typeof(IAttributeAccessor).FullName}");
            }

            Dictionary<string, object?> parsedAttributes = new();
            int attributesCount = reader.ReadMapHeader();

            for (int i = 0; i < attributesCount; i++) {
                string attributeName = reader.ReadString();
                AttributeDefinition attribute = abstractAttributeAccessor.GetAttribute(attributeName);
                bool attributeRequired = attribute.IsRequired;
                ValueResult attributeValueResult = DecodeValueWithContext(ref reader, attribute.Value, options, attributeRequired);

                if (!attributeValueResult.IsUnknown && attributeRequired && attributeValueResult.IsNull) {
                    options.Reports.Error($"The attribute \"{attributeName}\" is required and must not be null");
                }

                parsedAttributes.Add(attributeName, attributeValueResult.Value);
            }

            return ActivateComplex(definition, parsedAttributes);
        }

        private object DecodeNonNullableBlock(ref MessagePackReader reader, BlockDefinition definition, DecodingOptions options) =>
            DecodeNullableComplex(ref reader, definition, options) ?? throw new TerraformDynamicMessagePackDecodingException($"The MessagePack reader tried to reconstruct the Terraform block of type {definition.SourceType.FullName}, but the data delivered to the reader was empty{Environment.NewLine}{definition}");

        private object DecodeValue(ValueDefinition definition, ref MessagePackReader reader, DecodingOptions options) => definition.TypeConstraint switch {
            TerraformTypeConstraint.Dynamic => DecodeDynamic(ref reader, (DynamicDefinition)definition),
            TerraformTypeConstraint.String => DecodeString(ref reader),
            TerraformTypeConstraint.Number => DecodeNumber(ref reader, definition),
            TerraformTypeConstraint.Bool => DecodeBool(ref reader),
            TerraformTypeConstraint.List => DecodeList(ref reader, (MonoRangeDefinition)definition, options),
            TerraformTypeConstraint.Set => DecodeSet(ref reader, (MonoRangeDefinition)definition, options),
            TerraformTypeConstraint.Map => DecodeMap(ref reader, (MapDefinition)definition, options),
            TerraformTypeConstraint.Object => DecodeNullableComplex(ref reader, (ObjectDefinition)definition, options) ?? throw new TerraformDynamicMessagePackDecodingException($"The MessagePack reader tried to reconstruct the Terraform object of type {definition.SourceType.FullName}, but the data delivered to the reader was empty{Environment.NewLine}{definition}"),
            TerraformTypeConstraint.Block => DecodeNonNullableBlock(ref reader, (BlockDefinition)definition, options),
            _ => throw new NotImplementedException($"The decoding of this Terraform constraint type is not implemented: {definition.TypeConstraint}")
        };

        private ValueResult DecodeValueWithContext(ref MessagePackReader reader, ValueDefinition value, DecodingOptions options, bool isResultRequired = false)
        {
            object? value2;
            bool isNull;
            bool isUnknown;

            if (reader.TryReadNil()) {
                value2 = null;
                isNull = true;
                isUnknown = false;
            } else if (reader.NextMessagePackType == MessagePackType.Extension) {
                ExtensionResult extension = reader.ReadExtensionFormat();
                value2 = null;
                isNull = true;
                isUnknown = extension.TypeCode == 0;
            } else {
                value2 = DecodeValue(value, ref reader, options);
                isNull = false;
                isUnknown = false;
            }

            object? value3 = value.SourceTypeWrapping.Contains(TypeWrapping.TerraformValue)
                ? TerraformValue.CreateInstance(value.SourceType, !isResultRequired, value2, isNull, isUnknown)
                : value2;

            return new ValueResult(value3, isNull, isUnknown);
        }

        public object? DecodeDynamic(object? unknown, Type knownType, DecodingOptions options)
        {
            if (unknown is TerraformDynamic terraformDynamic) {
                ValueDefinition value = _schemaBuilder.BuildDynamic(terraformDynamic.Definition, knownType);
                MessagePackReader reader = new(terraformDynamic.Memory);
                return DecodeValueWithContext(ref reader, value, options);
            }

            return unknown;
        }

        /// <summary>
        /// Decodes the schema of a block.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="block"></param>
        /// <param name="options"></param>
        public object? DecodeNullableBlock(ref MessagePackReader reader, BlockDefinition block, DecodingOptions options)
        {
            if (options is null) {
                throw new ArgumentNullException(nameof(options));
            }

            return DecodeNullableComplex(ref reader, block, options);
        }

        /// <summary>
        /// Decodes the schema of a block.
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="block"></param>
        /// <param name="options"></param>
        public object? DecodeNullableBlock(ReadOnlyMemory<byte> memory, BlockDefinition block, DecodingOptions options)
        {
            MessagePackReader reader = new(memory);
            return DecodeNullableBlock(ref reader, block, options);
        }

        /// <summary>
        /// Decodes the schema of a block.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="block"></param>
        /// <param name="options"></param>
        public object DecodeBlock(ref MessagePackReader reader, BlockDefinition block, DecodingOptions options)
        {
            if (options is null) {
                throw new ArgumentNullException(nameof(options));
            }

            return DecodeNonNullableBlock(ref reader, block, options);
        }

        /// <summary>
        /// Decodes the schema of a block.
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="block"></param>
        /// <param name="options"></param>
        public object DecodeBlock(ReadOnlyMemory<byte> memory, BlockDefinition block, DecodingOptions options)
        {
            MessagePackReader reader = new(memory);
            return DecodeBlock(ref reader, block, options);
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
