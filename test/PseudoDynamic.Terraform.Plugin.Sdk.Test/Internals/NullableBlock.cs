using PseudoDynamic.Terraform.Plugin.Schema;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class NullableBlock : NullablePrimitivesBlock
    {
        //public static readonly BlockSchema DefaultSchema;

        //static NullableBlock()
        //{
        //    var builder = BlockSchemaBuilder.CreateDefault();
        //    builder.ConcatSchema(typeof(NullableBlock));
        //    DefaultSchema = builder.BuildSchema();
        //}

        /// <summary>
        /// Represents Terraform's "object".
        /// </summary>
        public ObjectAttributes Object { get; set; }

        /// <summary>
        /// Represents Terraform's "object".
        /// </summary>
        public IList<ObjectAttributes> ListOfObject { get; set; }

        /// <summary>
        /// Represents Terraform's "object".
        /// </summary>
        public ISet<ObjectAttributes> SetOfObject { get; set; }

        /// <summary>
        /// Represents Terraform's "object".
        /// </summary>
        public IDictionary<string, ObjectAttributes> MapOfOfObject { get; set; }

        /// <summary>
        /// Represents Terraform's "object".
        /// </summary>
        public ObjectAttributes? NullableObject { get; set; }

        /// <summary>
        /// Represents Terraform's "tuple".
        /// </summary>
        [Tuple]
        public TupleIndexes Tuple { get; set; }

        /// <summary>
        /// Represents Terraform's "tuple".
        /// </summary>
        [Tuple]
        public TupleIndexes? NullableTuple { get; set; }

        /// <summary>
        /// Represents Terraform's "block".
        /// </summary>
        [Block]
        public ObjectAttributes SingleNestedBlock { get; set; }

        /// <summary>
        /// Represents Terraform's "block".
        /// </summary>
        [Block]
        public IList<ObjectAttributes> ListOfNestedBlock { get; set; }

        /// <summary>
        /// Represents Terraform's "block".
        /// </summary>
        [Block]
        public ISet<ObjectAttributes> SetOfNestedBlock { get; set; }

        /// <summary>
        /// Represents Terraform's "block".
        /// </summary>
        [Block]
        public IDictionary<string, ObjectAttributes> MapOfNestedBlock { get; set; }

        /// <summary>
        /// Represents Terraform's "block".
        /// </summary>
        [Block]
        public ObjectAttributes NullableSingleNestedBlock { get; set; }

        public class ObjectAttributes : NullablePrimitivesBlock
        {
        }

        public class TupleIndexes
        {
        }
    }
}
