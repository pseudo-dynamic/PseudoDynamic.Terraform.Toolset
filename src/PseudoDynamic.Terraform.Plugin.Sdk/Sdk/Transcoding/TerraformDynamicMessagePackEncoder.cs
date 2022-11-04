using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using DotNext.Buffers;
using MessagePack;
using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    internal class TerraformDynamicMessagePackEncoder
    {
        private readonly struct TerraformValueEncoder : ITerraformValueMessagePackEncoder
        {
            private readonly TerraformDynamicMessagePackEncoder _encoder;

            internal TerraformValueEncoder(TerraformDynamicMessagePackEncoder encoder) =>
                _encoder = encoder;

            public void Encode<T>(ref MessagePackWriter writer, ValueDefinition value, ITerraformValue<T> terraformValue) =>
                _encoder.EncodeTerraformValue(ref writer, value, terraformValue);

            public void Encode(ref MessagePackWriter writer, ValueDefinition value, ITerraformValue terraformValue) =>
                _encoder.EncodeTerraformValue(ref writer, value, terraformValue);
        }

        internal delegate void EncodeDelegate<TDefinition, TContent>(TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, TDefinition value, [AllowNull] TContent content);

        private static readonly Type BigIntegerType = typeof(BigInteger);
        private static readonly TypeAccessor EncoderTypeAccessor = new TypeAccessor(typeof(TerraformDynamicMessagePackEncoder));
        private readonly static GenericTypeAccessor CollectionEncoderAccessor = new GenericTypeAccessor(typeof(CollectionEncoder<>));
        private readonly static GenericTypeAccessor DictionaryEncoderAccessor = new GenericTypeAccessor(typeof(DictionaryEncoder<,>));

        private static void EncodeValue<T>(TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, ValueDefinition value, [AllowNull] T content) =>
            encoder.EncodeValue(ref writer, value, content);

        private static ReadOnlyMemory<byte> EncodeUsingSparseBuffer<TDefinition, TContent>(TerraformDynamicMessagePackEncoder encoder, TDefinition value, TContent content, EncodeDelegate<TDefinition, TContent> encode)
        {
            using var bufferWriter = new SparseBufferWriter<byte>(); // TODO: estimate proper defaults
            var writer = new MessagePackWriter(bufferWriter);
            encode(encoder, ref writer, value, content);
            writer.Flush();
            var buffer = new byte[bufferWriter.WrittenCount];
            bufferWriter.CopyTo(buffer);
            return buffer;
        }

        private readonly SchemaBuilder _schemaBuilder;

        public TerraformDynamicMessagePackEncoder(SchemaBuilder schemaBuilder) =>
            _schemaBuilder = schemaBuilder ?? throw new ArgumentNullException(nameof(schemaBuilder));

        private void EncodeDynamic<T>(ref MessagePackWriter writer, DynamicDefinition definition, T content)
        {
            if (content is TerraformDynamic dynamic) {
                if (dynamic.Encoding != TerraformDynamicEncoding.MessagePack) {
                    throw new TerraformDynamicMessagePackEncodingException($"An encoding type other than {TerraformDynamicEncoding.MessagePack} is not supported");
                }

                writer.WriteRaw(dynamic.Memory.Span);
            }

            if (content is null) {
                EncodeValue(ref writer, definition, content);
                return;
            }

            var resolvedDefinition = _schemaBuilder.BuildDynamic(definition, content.GetType());
            EncodeValue(ref writer, resolvedDefinition, content);
        }

        private void EncodeString<T>(ref MessagePackWriter writer, ValueDefinition _, T content) =>
            writer.Write(Unsafe.As<string>(content)); // Writes implicitly UTF8

        private void EncodeNumber<T>(ref MessagePackWriter writer, ValueDefinition value, T content)
        {
            var sourceType = value.SourceType;

            switch (Type.GetTypeCode(value.SourceType)) {
                case TypeCode.Byte:
                    writer.Write(Unsafe.As<T, byte>(ref content));
                    break;
                case TypeCode.SByte:
                    writer.Write(Unsafe.As<T, sbyte>(ref content));
                    break;
                case TypeCode.UInt16:
                    writer.Write(Unsafe.As<T, ushort>(ref content));
                    break;
                case TypeCode.Int16:
                    writer.Write(Unsafe.As<T, short>(ref content));
                    break;
                case TypeCode.UInt32:
                    writer.Write(Unsafe.As<T, uint>(ref content));
                    break;
                case TypeCode.Int32:
                    writer.Write(Unsafe.As<T, int>(ref content));
                    break;
                case TypeCode.UInt64:
                    writer.Write(Unsafe.As<T, ulong>(ref content));
                    break;
                case TypeCode.Int64:
                    writer.Write(Unsafe.As<T, long>(ref content));
                    break;
                case TypeCode.Single:
                    writer.Write(Unsafe.As<T, float>(ref content));
                    break;
                case TypeCode.Double:
                    writer.Write(Unsafe.As<T, double>(ref content));
                    break;
                case TypeCode.Object:
                    if (sourceType == BigIntegerType) {
                        ref var bigInteger = ref Unsafe.As<T, BigInteger>(ref content);
                        // Write number as number in base 10 notation
                        writer.Write(bigInteger.ToString());
                    } else {
                        // If no match
                        goto default;
                    }

                    break;
                default:
                    throw new TerraformDynamicMessagePackEncodingException($"The C# type {sourceType.FullName} is not representable as Terraform number");
            }
        }

        private void EncodeBool<T>(ref MessagePackWriter writer, ValueDefinition _, T content) =>
            writer.Write(Unsafe.As<T, bool>(ref content));

        private void EncodeMonoRange<T>(ref MessagePackWriter writer, MonoRangeDefinition range, T content)
        {
            var collectionEncoder = (ICollectionEncoder)CollectionEncoderAccessor
                .MakeGenericTypeAccessor(range.Item.SourceType)
                .CreateInstance(x => x.GetPublicInstanceActivator, content);

            collectionEncoder.Encode(this, ref writer, range.Item);
        }

        private void EncodeMonoRange<TElement>(ref MessagePackWriter writer, ValueDefinition item, ICollection<TElement> collection)
        {
            writer.WriteArrayHeader(collection.Count);

            foreach (var element in collection) {
                EncodeValue(ref writer, item, element);
            }
        }

        private void EncodeMap(ref MessagePackWriter writer, MapDefinition dictionary, object content)
        {
            var dictionaryEncoder = (IDictionaryEncoder)DictionaryEncoderAccessor
                .MakeGenericTypeAccessor(dictionary.Key.SourceType, dictionary.Value.SourceType)
                .CreateInstance(x => x.GetPublicInstanceActivator, content);

            dictionaryEncoder.Encode(this, ref writer, dictionary.Key, dictionary.Value);
        }

        private void EncodeMap<TKey, TValue>(ref MessagePackWriter writer, ValueDefinition key, ValueDefinition value, ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            writer.WriteMapHeader(collection.Count);

            foreach (var tuple in collection) {
                EncodeValue(ref writer, key, tuple.Key);
                EncodeValue(ref writer, value, tuple.Value);
            }
        }

        private void EncodeComplex<T>(ref MessagePackWriter writer, ComplexDefinition complex, T content)
        {
            if (complex is not IAttributeAccessor abstractAttributeAccessor) {
                throw new NotImplementedException($"Complex definition does not implement {typeof(IAttributeAccessor).FullName}");
            }

            writer.WriteMapHeader(abstractAttributeAccessor.Count);
            var contentAccessor = FastMember.ObjectAccessor.Create(content);

            foreach (var attribute in abstractAttributeAccessor.GetEnumerator()) {
                writer.Write(attribute.Name);
                var contentAccessedObject = contentAccessor[attribute.AttributeReflectionMetadata.Property.Name];

                EncodeValueDelegateCompiler.RequestCompiledEncodeValueDelegate(contentAccessedObject?.GetType() ?? typeof(object))
                        .Invoke(this, ref writer, attribute.Value, contentAccessedObject);
            }
        }

        private void EncodeNonWrappedlValue<T>(ref MessagePackWriter writer, ValueDefinition value, [DisallowNull] T content)
        {
            switch (value.TypeConstraint) {
                case TerraformTypeConstraint.Dynamic:
                    EncodeDynamic(ref writer, (DynamicDefinition)value, content);
                    break;
                case TerraformTypeConstraint.String:
                    EncodeString(ref writer, value, content);
                    break;
                case TerraformTypeConstraint.Number:
                    EncodeNumber(ref writer, value, content);
                    break;
                case TerraformTypeConstraint.Bool:
                    EncodeBool(ref writer, value, content);
                    break;
                case TerraformTypeConstraint.List or TerraformTypeConstraint.Set:
                    EncodeMonoRange(ref writer, (MonoRangeDefinition)value, content);
                    break;
                case TerraformTypeConstraint.Map:
                    EncodeMap(ref writer, (MapDefinition)value, content);
                    break;
                case TerraformTypeConstraint.Object:
                    EncodeComplex(ref writer, (ObjectDefinition)value, content);
                    break;
                case TerraformTypeConstraint.Tuple:
                    throw new NotImplementedException($"The encoding of Terraform constraint type {TerraformTypeConstraint.Tuple} is not implemented");
                case TerraformTypeConstraint.Block:
                    EncodeComplex(ref writer, (BlockDefinition)value, content);
                    break;
                default:
                    throw new NotImplementedException($"The encoding of Terraform constraint type {value.TypeConstraint} is not supported");
            }
        }

        private bool TryEncodeTerraformValue(ref MessagePackWriter writer, ValueDefinition _, ITerraformValue terraformValue)
        {
            if (terraformValue.IsUnknown) {
                writer.WriteExtensionFormat(new ExtensionResult(0, ReadOnlySequence<byte>.Empty));
                return true;
            }

            if (terraformValue.IsNull) {
                writer.WriteNil();
                return true;
            }

            return false;
        }

        private void EncodeTerraformValue(ref MessagePackWriter writer, ValueDefinition value, ITerraformValue terraformValue)
        {
            if (TryEncodeTerraformValue(ref writer, value, terraformValue)) {
                return;
            }

            EncodeDynamic(ref writer, (DynamicDefinition)value, terraformValue.Value!);
        }

        private void EncodeTerraformValue<T>(ref MessagePackWriter writer, ValueDefinition value, ITerraformValue<T> terraformValue)
        {
            if (TryEncodeTerraformValue(ref writer, value, terraformValue)) {
                return;
            }

            EncodeNonWrappedlValue(ref writer, value, terraformValue.Value!); // T is preserved
        }

        private void EncodeTerraformValue<T>(ref MessagePackWriter writer, ValueDefinition value, T content)
        {
            if (content is not ITerraformValue terraformValue) {
                throw new TerraformDynamicMessagePackEncodingException($"An object of type {typeof(ITerraformValue)} was expected");
            }

            terraformValue.Encode(new TerraformValueEncoder(this), ref writer, value);
        }

        protected internal virtual void EncodeValue<T>(ref MessagePackWriter writer, ValueDefinition value, [AllowNull] T content)
        {
            if (content is null) {
                writer.WriteNil();
                return;
            }

            if (value.SourceTypeWrapping.Contains(TypeWrapping.TerraformValue)) {
                EncodeTerraformValue(ref writer, value, content);
                return;
            }

            EncodeNonWrappedlValue(ref writer, value, content);
        }

        public ReadOnlyMemory<byte> EncodeValue<T>(ValueDefinition value, [AllowNull] T content) =>
            EncodeUsingSparseBuffer(this, value, content, EncodeValue);

        public ReadOnlyMemory<byte> EncodeValue(ValueDefinition value, object? content) =>
            EncodeUsingSparseBuffer(this, value, content, static (TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, ValueDefinition value, object? content) =>
                EncodeValueDelegateCompiler.RequestCompiledEncodeValueDelegate(value.SourceType)(encoder, ref writer, value, content));

        private static class EncodeValueDelegateCompiler
        {
            internal delegate void EncodeValueDelegate(TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, ValueDefinition value, object? content);

            private readonly static MethodAccessor EncodeValueMethod = EncoderTypeAccessor.GetMethodAccessor(static x => x.GetPrivateStaticMethod, nameof(EncodeValue));

            private static EncodeValueDelegate CompileEncodeValueDelegate(Type sourceType)
            {
                var method = EncodeValueMethod.MakeGenericMethod(sourceType);
                ParameterExpression param1 = Expression.Parameter(typeof(TerraformDynamicMessagePackEncoder), "encoder");
                ParameterExpression param2 = Expression.Parameter(typeof(MessagePackWriter).MakeByRefType(), "writer");
                ParameterExpression param3 = Expression.Parameter(typeof(ValueDefinition), "value");
                ParameterExpression param4 = Expression.Parameter(typeof(object), "content");

                MethodCallExpression body = Expression.Call(instance: null, method, param1, param2, param3, sourceType.IsValueType
                    ? Expression.Unbox(param4, sourceType)
                    : Expression.Convert(param4, sourceType));

                return Expression.Lambda<EncodeValueDelegate>(body, param1, param2, param3, param4).Compile();
            }

            private static readonly Dictionary<Type, EncodeValueDelegate> EncodeValueCompiledDelegates = new();

            internal static EncodeValueDelegate RequestCompiledEncodeValueDelegate(Type sourceType)
            {
                if (!EncodeValueCompiledDelegates.TryGetValue(sourceType, out var encodeValueDelegate)) {
                    encodeValueDelegate = CompileEncodeValueDelegate(sourceType);
                    EncodeValueCompiledDelegates[sourceType] = encodeValueDelegate;
                }

                return encodeValueDelegate;
            }
        }

        private interface ICollectionEncoder
        {
            void Encode(TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, ValueDefinition item);
        }

        private class CollectionEncoder<TItem> : ICollectionEncoder
        {
            public ICollection<TItem> Collection { get; }

            public CollectionEncoder(ICollection<TItem> collection) =>
                Collection = collection;

            public virtual void Encode(TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, ValueDefinition item) =>
                encoder.EncodeMonoRange(ref writer, item, Collection);
        }

        private interface IDictionaryEncoder
        {
            void Encode(TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, ValueDefinition key, ValueDefinition value);
        }

        private class DictionaryEncoder<TKey, TValue> : IDictionaryEncoder
        {
            public ICollection<KeyValuePair<TKey, TValue>> Collection { get; }

            public DictionaryEncoder(ICollection<KeyValuePair<TKey, TValue>> collection) =>
                Collection = collection;

            public virtual void Encode(TerraformDynamicMessagePackEncoder encoder, ref MessagePackWriter writer, ValueDefinition key, ValueDefinition value) =>
                encoder.EncodeMap(ref writer, key, value, Collection);
        }
    }
}
