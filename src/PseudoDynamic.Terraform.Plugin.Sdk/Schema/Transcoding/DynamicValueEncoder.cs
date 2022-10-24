using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using DotNext.Buffers;
using MessagePack;
using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph;
using ObjectAccessor = FastMember.ObjectAccessor;

namespace PseudoDynamic.Terraform.Plugin.Schema.Transcoding
{
    internal class DynamicValueEncoder
    {
        internal readonly static DynamicValueEncoder Default = new DynamicValueEncoder();

        private readonly static TypeAccessor DynamicValueEncoderAccessor = new TypeAccessor(typeof(DynamicValueEncoder));
        private readonly static GenericTypeAccessor CollectionEncoderAccessor = new GenericTypeAccessor(typeof(CollectionEncoder<>));
        private readonly static GenericTypeAccessor DictionaryEncoderAccessor = new GenericTypeAccessor(typeof(DictionaryEncoder<,>));

        private readonly static Dictionary<Type, EncodeTerraformValueCompiledMethod> EncodeTerraformValueCompiledMethods = new Dictionary<Type, EncodeTerraformValueCompiledMethod>();

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DynamicValueEncoder))]
        private static void EncodeTerraformValue<T>(DynamicValueEncoder encoder, ref MessagePackWriter writer, ValueDefinition value, ITerraformValue<T> terraformValue)
        {
            if (terraformValue.IsUnknown) {
                writer.WriteExtensionFormat(new ExtensionResult(0, ReadOnlySequence<byte>.Empty));
                return;
            }

            if (terraformValue.IsNull) {
                writer.WriteNil();
                return;
            }

            encoder.EncodeNonNullValue(ref writer, value, terraformValue.Value);
        }

        private static EncodeTerraformValueCompiledMethod.EncodeTerraformValueDelegate GetEncodeTerraformValueDelegate(Type terraformValueType, Type valueType)
        {
            if (EncodeTerraformValueCompiledMethods.TryGetValue(terraformValueType, out var compiledMethod)) {
                return compiledMethod.EncodeTerraformValue;
            }

            compiledMethod = new EncodeTerraformValueCompiledMethod(terraformValueType, valueType);
            EncodeTerraformValueCompiledMethods[terraformValueType] = compiledMethod;
            return compiledMethod.EncodeTerraformValue;
        }

        public void EncodeNumber(ref MessagePackWriter writer, ValueDefinition value, object content)
        {
            var sourceType = value.SourceType;

            switch (Type.GetTypeCode(value.SourceType)) {
                case TypeCode.Byte:
                    writer.Write((byte)content);
                    break;
                case TypeCode.SByte:
                    writer.Write((sbyte)content);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)content);
                    break;
                case TypeCode.Int16:
                    writer.Write((short)content);
                    break;
                case TypeCode.UInt32:
                    writer.Write((uint)content);
                    break;
                case TypeCode.Int32:
                    writer.Write((int)content);
                    break;
                case TypeCode.Single:
                    writer.Write((float)content);
                    break;
                case TypeCode.UInt64:
                    writer.Write((ulong)content);
                    break;
                case TypeCode.Int64:
                    writer.Write((long)content);
                    break;
                case TypeCode.Double:
                    writer.Write((double)content);
                    break;
                case TypeCode.Object:
                    if (sourceType == typeof(BigInteger)) {
                        var bigInteger = (BigInteger)content;
                        // Write number as number in base 10 notation
                        writer.Write(bigInteger.ToString());
                    } else {
                        goto default;
                    }

                    break;
                default:
                    throw new DynamicValueEncodingException($"The C# type {sourceType.FullName} cannot be encoded to a Terraform number");
            }
        }

        private void EncodeMonoRange(ref MessagePackWriter writer, MonoRangeDefinition range, object content)
        {
            var collectionEncoder = (ICollectionEncoder)CollectionEncoderAccessor.MakeGenericTypeAccessor(range.Item.SourceType).CreateInstance(x => x.GetPublicInstanceActivator, content);
            collectionEncoder.WriteHeader(ref writer);
            var itemEnumerator = collectionEncoder.GetEnumerator(range.Item);

            while (itemEnumerator.MoveNext()) {
                EncodeValue(ref writer, range.Item, itemEnumerator.Current);
            }
        }

        private void EncodeMap(ref MessagePackWriter writer, MapDefinition dictionary, object content)
        {
            var dictionaryEncoder = (IDictionaryEncoder)DictionaryEncoderAccessor
                .MakeGenericTypeAccessor(dictionary.Key.SourceType, dictionary.Value.SourceType)
                .CreateInstance(x => x.GetPublicInstanceActivator, content);

            dictionaryEncoder.WriteHeader(ref writer);
            var itemEnumerator = dictionaryEncoder.GetEnumerator();

            while (itemEnumerator.MoveNext()) {
                writer.Write((string)itemEnumerator.Current!); // We never expect key being null

                if (!itemEnumerator.MoveNext()) {
                    throw new DynamicValueEncodingException("A criticial failure happend because the value after key was missing while encoding a map entry");
                }

                EncodeValue(ref writer, dictionary.Value, itemEnumerator.Current); // The value of its corresponding key
            }
        }

        private void EncodeComplex(ref MessagePackWriter writer, ComplexDefinition complex, object content)
        {
            if (complex is not IAttributeAccessor abstractAttributeAccessor) {
                throw new NotImplementedException($"Complex definition does not implement {typeof(IAttributeAccessor).FullName}");
            }

            writer.WriteMapHeader(abstractAttributeAccessor.Count);
            var contentAccessor = ObjectAccessor.Create(content);

            foreach (var attribute in abstractAttributeAccessor.GetEnumerator()) {
                writer.Write(attribute.Name);
                EncodeValue(ref writer, attribute.Value, contentAccessor[attribute.AttributeReflectionMetadata.Property.Name]);
            }
        }

        public void EncodeNonNullValue(ref MessagePackWriter writer, ValueDefinition value, object content)
        {
            switch (value.TypeConstraint) {
                case TerraformTypeConstraint.Any:
                    throw new NotImplementedException($"The encoding of Terraform constraint type {TerraformTypeConstraint.Any} is not implemented");
                case TerraformTypeConstraint.String:
                    writer.Write((string)content);
                    break;
                case TerraformTypeConstraint.Number:
                    EncodeNumber(ref writer, value, content);
                    break;
                case TerraformTypeConstraint.Bool:
                    writer.Write((bool)content); // Writes implicitly UTF8
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

        public void EncodeValue(ref MessagePackWriter writer, ValueDefinition value, object? content)
        {
            if (content is null) {
                writer.WriteNil();
                return;
            }

            if (value.IsWrappedByTerraformValue) {
                GetEncodeTerraformValueDelegate(value.OuterType, value.SourceType).Invoke(this, ref writer, value, content);
                return;
            }

            EncodeNonNullValue(ref writer, value, content);
        }

        public void EncodeSchema(ref MessagePackWriter writer, BlockDefinition block, object content)
        {
            if (content is null) {
                throw new DynamicValueEncodingException("The very first encoding schema cannot be null");
            }

            EncodeComplex(ref writer, block, content);
        }

        public ReadOnlyMemory<byte> EncodeSchema(BlockDefinition block, object content)
        {
            using var bufferWriter = new SparseBufferWriter<byte>(); // TODO: estimate proper defaults
            var writer = new MessagePackWriter(bufferWriter);
            EncodeSchema(ref writer, block, content);
            writer.Flush();
            var buffer = new byte[bufferWriter.WrittenCount];
            bufferWriter.CopyTo(buffer);
            return buffer;
        }

        private interface ICollectionEncoder
        {
            void WriteHeader(ref MessagePackWriter writer);
            IEnumerator<object?> GetEnumerator(TerraformDefinition item);
        }

        private class CollectionEncoder<TItem> : ICollectionEncoder
        {
            public ICollection<TItem> Collection { get; }

            public CollectionEncoder(ICollection<TItem> collection) =>
                Collection = collection;

            public virtual void WriteHeader(ref MessagePackWriter writer) =>
                writer.WriteArrayHeader(Collection.Count);

            public virtual IEnumerator<object?> GetEnumerator(TerraformDefinition item)
            {
                var enumerator = Collection.GetEnumerator();

                if (!item.SourceType.IsValueType) {
                    return Unsafe.As<IEnumerator<object>>(Collection.GetEnumerator());
                }

                return Cast();

                IEnumerator<object?> Cast()
                {
                    while (enumerator.MoveNext()) {
                        yield return enumerator.Current;
                    }
                }
            }
        }

        private interface IDictionaryEncoder
        {
            void WriteHeader(ref MessagePackWriter writer);
            IEnumerator<object?> GetEnumerator();
        }

        private class DictionaryEncoder<TKey, TValue> : CollectionEncoder<KeyValuePair<TKey, TValue>>, IDictionaryEncoder
        {
            public DictionaryEncoder(ICollection<KeyValuePair<TKey, TValue>> collection) : base(collection)
            {
            }

            public override void WriteHeader(ref MessagePackWriter writer) =>
                writer.WriteMapHeader(Collection.Count);

            public virtual IEnumerator<object?> GetEnumerator()
            {
                var enumerator = Collection.GetEnumerator();

                while (enumerator.MoveNext()) {
                    yield return enumerator.Current.Key;
                    yield return enumerator.Current.Value;
                }
            }
        }

        private class EncodeTerraformValueCompiledMethod
        {
            internal delegate void EncodeTerraformValueDelegate(DynamicValueEncoder encoder, ref MessagePackWriter writer, ValueDefinition value, object terraformValue);

            private readonly static MethodAccessor EncodeTerraformValueAccessor = DynamicValueEncoderAccessor.GetMethodAccessor(static x => x.GetPrivateStaticMethod, nameof(EncodeTerraformValue));

            public EncodeTerraformValueDelegate EncodeTerraformValue { get; }

            public EncodeTerraformValueCompiledMethod(Type terraformValueType, Type valueType)
            {
                var method = EncodeTerraformValueAccessor.MakeGenericMethod(valueType);
                ParameterExpression param1 = Expression.Parameter(typeof(DynamicValueEncoder), "encoder");
                ParameterExpression param2 = Expression.Parameter(typeof(MessagePackWriter).MakeByRefType(), "writer");
                ParameterExpression param3 = Expression.Parameter(typeof(ValueDefinition), "value");
                ParameterExpression param4 = Expression.Parameter(typeof(object), "terraformValue");

                MethodCallExpression body = Expression.Call(
                    null,
                    method,
                    param1,
                    param2,
                    param3,
                    terraformValueType.IsValueType ? Expression.Unbox(param4, terraformValueType) : Expression.Convert(param4, terraformValueType));

                EncodeTerraformValue = Expression.Lambda<EncodeTerraformValueDelegate>(body, param1, param2, param3, param4).Compile();
            }
        }
    }
}
