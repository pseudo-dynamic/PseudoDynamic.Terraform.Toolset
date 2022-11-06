using CSF.Collections;
using PseudoDynamic.Terraform.Plugin.Reflection;
using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure.Fakes
{
    internal class SchemaFake<T>
    {
        public static readonly GenericTypeAccessor GenericListEqualityComparerAccessor = new GenericTypeAccessor(typeof(ListEqualityComparer<>));
        public static readonly GenericTypeAccessor GenericSetEqualityComparerAccessor = new GenericTypeAccessor(typeof(SetEqualityComparer<>));
        public static readonly GenericTypeAccessor GenericKeyValuePairAccessor = new GenericTypeAccessor(typeof(KeyValuePair<,>));

        static IEqualityComparer<T> GetDefaultEqualityComparer()
        {
            var typeOfT = typeof(T);

            if (typeOfT.IsImplementingGenericTypeDefinition(typeof(IList<>), out _, out var genericTypeArguments)) {
                return (IEqualityComparer<T>)GenericListEqualityComparerAccessor.MakeGenericTypeAccessor(genericTypeArguments).CreateInstance(x => x.GetPublicInstanceActivator);
            } else if (typeOfT.IsImplementingGenericTypeDefinition(typeof(ISet<>), out _, out genericTypeArguments)) {
                return (IEqualityComparer<T>)GenericSetEqualityComparerAccessor.MakeGenericTypeAccessor(genericTypeArguments).CreateInstance(x => x.GetPublicInstanceActivator);
            } else if (typeOfT.IsImplementingGenericTypeDefinition(typeof(IDictionary<,>), out _, out genericTypeArguments)) {
                var keyValuePairType = GenericKeyValuePairAccessor.MakeGenericTypeAccessor(genericTypeArguments).Type;
                return (IEqualityComparer<T>)GenericListEqualityComparerAccessor.MakeGenericTypeAccessor(keyValuePairType).CreateInstance(x => x.GetPublicInstanceActivator);
            } else {
                return EqualityComparer<T>.Default;
            }
        }

        public bool IsNestedBlock { get; init; }

        public T Value { get; }

        public Block Schema {
            get {
                var schema = _schema;

                if (schema is not null) {
                    return schema;
                }

                if (IsNestedBlock) {
                    schema = new NestedBlock(Value, _equalityComparer);
                } else {
                    schema = new Block(Value, _equalityComparer);
                }

                _schema = schema;
                return schema;
            }
        }

        private readonly IEqualityComparer<T>? _equalityComparer;
        private SchemaFake<T>.Block? _schema;

        public SchemaFake(T value, IEqualityComparer<T>? equalityComparer)
        {
            Value = value;
            _equalityComparer = equalityComparer ?? GetDefaultEqualityComparer();
        }

        public SchemaFake(T value) : this(value, null)
        {
        }

        [Block]
        internal class Block : ISchemaFake
        {
            public static Block OfValue(T value) =>
                new Block(value);

            public static SchemaFake<IList<T>>.Block HavingList(params T[] values) =>
                new SchemaFake<IList<T>>.Block(values.ToList(), new ListEqualityComparer<T>());

            public static SchemaFake<ISet<T>>.Block HavingSet(params T[] values) =>
                new SchemaFake<ISet<T>>.Block(values.ToHashSet(), new SetEqualityComparer<T>());

            public static IList<Block> RangeList(params T[] values) =>
                values.Select(OfValue).ToList();

            public static ISet<Block> RangeSet(params T[] values) =>
                values.Select(OfValue).ToHashSet();

            public static IDictionary<TKey, Block> RangeMap<TKey>(params (TKey Key, T Value)[] values)
                where TKey : notnull =>
                values.ToDictionary(x => x.Key, x => OfValue(x.Value));

            private readonly IEqualityComparer<T> _equalityComparer;

            public virtual T Value { get; }

            object? ISchemaFake.Value => Value;

            internal Block(T value, IEqualityComparer<T>? equalityComparer)
            {
                Value = value;
                _equalityComparer = equalityComparer ?? GetDefaultEqualityComparer();
            }

            public Block(T value) : this(value, null) =>
                Value = value;

            public override bool Equals(object? obj) =>
                obj is Block other
                && _equalityComparer.Equals(Value, other.Value);

            public override int GetHashCode() =>
                Value is null ? 0 : HashCode.Combine(_equalityComparer.GetHashCode(Value));
        }

        [Object]
        public class Object : Block
        {
            public new static Object OfValue(T value) =>
                new Object(value);

            public new static SchemaFake<IList<T>>.Object HavingList(params T[] values) =>
                new SchemaFake<IList<T>>.Object(values.ToList(), new ListEqualityComparer<T>());

            public new static SchemaFake<ISet<T>>.Object HavingSet(params T[] values) =>
                new SchemaFake<ISet<T>>.Object(values.ToHashSet(), new SetEqualityComparer<T>());

            public new static IList<Object> RangeList(params T[] values) =>
                values.Select(OfValue).ToList();

            public override T Value => base.Value;

            public Object(T value) : base(value)
            {
            }

            internal Object(T value, IEqualityComparer<T>? equalityComparer) : base(value, equalityComparer)
            {
            }
        }

        [Block]
        public class NestedBlock : Block
        {
            public new static NestedBlock OfValue(T value) =>
                new NestedBlock(value);

            public new static SchemaFake<IList<T>>.NestedBlock HavingList(params T[] values) =>
                new SchemaFake<IList<T>>.NestedBlock(values.ToList(), new ListEqualityComparer<T>());

            public new static SchemaFake<ISet<T>>.NestedBlock HavingSet(params T[] values) =>
                new SchemaFake<ISet<T>>.NestedBlock(values.ToHashSet(), new SetEqualityComparer<T>());

            public new static IList<NestedBlock> RangeList(params T[] values) =>
                values.Select(OfValue).ToList();

            public new static ISet<NestedBlock> RangeSet(params T[] values) =>
                values.Select(OfValue).ToHashSet();

            [NestedBlock]
            public override T Value => base.Value;

            internal NestedBlock(T value, IEqualityComparer<T>? equalityComparer) : base(value, equalityComparer)
            {
            }

            public NestedBlock(T value) : base(value)
            {
            }
        }

        public class TerraformValueFake : SchemaFake<ITerraformValue<T>>
        {
            private TerraformValueFake(ITerraformValue<T> value, IEqualityComparer<ITerraformValue<T>>? equalityComparer) : base(value, equalityComparer)
            {
            }

            public TerraformValueFake(ITerraformValue<T> value, IEqualityComparer<T>? equalityComparer)
                : this(value, new TerraformEqualityEqualityComparer<T>(equalityComparer ?? SchemaFake<T>.GetDefaultEqualityComparer()))
            {
            }

            public TerraformValueFake(ITerraformValue<T> value)
                : this(value, default(IEqualityComparer<T>))
            {
            }
        }
    }
}
