using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    [Serializable]
    internal class BlockException : Exception
    {
        public BlockException()
        {
        }

        public BlockException(string? message) : base(message)
        {
        }

        public BlockException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BlockException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
