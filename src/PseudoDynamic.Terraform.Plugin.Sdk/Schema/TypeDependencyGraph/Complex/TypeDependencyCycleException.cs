using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Complex
{
    [Serializable]
    internal class TypeDependencyCycleException : Exception
    {
        public TypeDependencyCycleException()
        {
        }

        public TypeDependencyCycleException(string? message) : base(message)
        {
        }

        public TypeDependencyCycleException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TypeDependencyCycleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
