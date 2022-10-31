using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    [Serializable]
    internal class TypeDependencyCycleException : Exception
    {
        public Type? Type { get; init; }

        public override string Message => (base.Message == string.Empty ? $"A type dependency cycle has been detected" : base.Message)
            + (Type != null ? $"{Environment.NewLine}Type = {Type.FullName}" : string.Empty);

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
