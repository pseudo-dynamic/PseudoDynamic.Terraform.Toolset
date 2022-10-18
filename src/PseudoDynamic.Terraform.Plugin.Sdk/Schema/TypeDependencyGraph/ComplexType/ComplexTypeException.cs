using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal class ComplexTypeException : Exception
    {
        public ComplexTypeException()
        {
        }

        public ComplexTypeException(string? message) : base(message)
        {
        }

        public ComplexTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ComplexTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
