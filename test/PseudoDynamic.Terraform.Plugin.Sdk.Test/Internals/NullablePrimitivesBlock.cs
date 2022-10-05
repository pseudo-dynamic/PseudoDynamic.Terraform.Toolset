using System.Numerics;

namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class NullablePrimitivesBlock
    {
        /// <summary>
        /// Represents "number".
        /// </summary>
        public sbyte SignedByte { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public sbyte Byte { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public ushort UnsignedShort { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public short Short { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public uint UnsignedInteger { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public int Integer { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public int? NullableInteger { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public ulong UnsignedLong { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public long Long { get; set; }

        /// <summary>
        /// Represents "number".
        /// </summary>
        public BigInteger BigInteger { get; set; }

        /// <summary>
        /// Represents "string".
        /// </summary>
        public string String { get; set; }

        /// <summary>
        /// Represents "string".
        /// </summary>
        public string? NullableString { get; set; }

        /// <summary>
        /// Represents "bool".
        /// </summary>
        public bool Bool { get; set; }

        /// <summary>
        /// Represents "bool".
        /// </summary>
        public bool? NullableBool { get; set; }

        /// <summary>
        /// Represents "dynamic".
        /// </summary>
        public object Any { get; set; }

        /// <summary>
        /// Represents "dynamic".
        /// </summary>
        public object? NullableAny { get; set; }

        /// <summary>
        /// Represents "list(string)".
        /// </summary>
        public IList<string> ListOfString { get; set; }

        /// <summary>
        /// Represents "list(string)".
        /// </summary>
        public IList<string>? NullableListOfString { get; set; }

        /// <summary>
        /// Represents "set(string)".
        /// </summary>
        public ISet<string> SetOfString { get; set; }

        /// <summary>
        /// Represents "set(string)".
        /// </summary>
        public ISet<string>? NullableSetOfString { get; set; }

        /// <summary>
        /// Represents "map(string)".
        /// </summary>
        public IDictionary<string, string> MapOfString { get; set; }

        /// <summary>
        /// Represents "map(string)".
        /// </summary>
        public IDictionary<string, string>? NullableMapOfString { get; set; }
    }
}
