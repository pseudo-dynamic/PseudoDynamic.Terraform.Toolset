using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    [Serializable]
    internal class NestedBlockException : BlockException
    {
        public NestedBlockException()
        {
        }

        public NestedBlockException(string? message) : base(message)
        {
        }

        public NestedBlockException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NestedBlockException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
