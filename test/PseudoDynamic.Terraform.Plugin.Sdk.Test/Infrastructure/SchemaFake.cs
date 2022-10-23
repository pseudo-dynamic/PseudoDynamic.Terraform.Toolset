using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal class SchemaFake<T>
    {
        public bool IsNestedBlock { get; init; }

        public T Value { get; }

        public ValueHavingSchema Schema
        {
            get
            {
                var schema = _schema;

                if (schema is not null)
                {
                    return schema;
                }

                if (IsNestedBlock)
                {
                    schema = new BlockHavingSchema(Value, _equalityComparer);
                }
                else
                {
                    schema = new ValueHavingSchema(Value, _equalityComparer);
                }

                _schema = schema;
                return schema;
            }
        }

        private readonly IEqualityComparer<T>? _equalityComparer;
        private SchemaFake<T>.ValueHavingSchema _schema;

        public SchemaFake(T value, IEqualityComparer<T>? equalityComparer)
        {
            Value = value;
            _equalityComparer = equalityComparer;
        }

        public SchemaFake(T value) : this(value, null)
        {
        }

        [Block]
        internal class ValueHavingSchema : ISchemaFake
        {
            public static ValueHavingSchema OfValue(T value) =>
                new ValueHavingSchema(value);

            private readonly IEqualityComparer<T> _equalityComparer;

            public virtual T Value { get; }

            object ISchemaFake.Value => Value;

            internal ValueHavingSchema(T value, IEqualityComparer<T>? equalityComparer)
            {
                Value = value;
                _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            }

            public ValueHavingSchema(T value)
                : this(value, EqualityComparer<T>.Default) =>
                Value = value;

            public override bool Equals(object? obj) =>
                obj is ValueHavingSchema other
                && _equalityComparer.Equals(Value, other.Value);

            public override int GetHashCode() =>
                Value is null ? 0 : HashCode.Combine(_equalityComparer.GetHashCode(Value));
        }

        [Object]
        public class ObjectBeingSchema : ValueHavingSchema
        {
            public static ObjectBeingSchema OfValue(T value) =>
                new ObjectBeingSchema(value);

            public override T Value => base.Value;

            public ObjectBeingSchema(T value) : base(value)
            {
            }

            internal ObjectBeingSchema(T value, IEqualityComparer<T>? equalityComparer) : base(value, equalityComparer)
            {
            }
        }

        [Block]
        public class BlockHavingSchema : ValueHavingSchema
        {
            [NestedBlock]
            public override T Value => base.Value;

            public BlockHavingSchema(T value) : base(value)
            {
            }

            internal BlockHavingSchema(T value, IEqualityComparer<T>? equalityComparer) : base(value, equalityComparer)
            {
            }
        }

        public class TerraformValue : SchemaFake<ITerraformValue<T>>
        {
            public TerraformValue(ITerraformValue<T> value, IEqualityComparer<ITerraformValue<T>>? equalityComparer) : base(value, equalityComparer)
            {
            }

            public TerraformValue(ITerraformValue<T> value, IEqualityComparer<T>? equalityComparer) : this(value, new TerraformEqualityEqualityComparer<T>(equalityComparer))
            {
            }

            public TerraformValue(ITerraformValue<T> value) : this(value, equalityComparer: default(IEqualityComparer<ITerraformValue<T>>))
            {
            }

            public TerraformValue(T value) : this(new TerraformValue<T>() { Value = value }, default(IEqualityComparer<ITerraformValue<T>>))
            {
            }
        }
    }
}