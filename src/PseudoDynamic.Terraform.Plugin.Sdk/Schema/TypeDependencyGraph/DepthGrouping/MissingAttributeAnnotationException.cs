using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.DepthGrouping
{
    public class MissingAttributeAnnotationException : Exception
    {
        public Type? ReceiverType { get; init; }
        public Type? MissingAttributeType { get; init; }

        public override string Message => (base.Message == string.Empty ? $"An attribute annotation was missing" : base.Message)
            + (ReceiverType != null ? $"{Environment.NewLine}Receiver Type = {ReceiverType.FullName}" : string.Empty)
            + (MissingAttributeType != null ? $"{Environment.NewLine}Missing Attribute Type = {MissingAttributeType.FullName}" : string.Empty);

        public MissingAttributeAnnotationException()
        {
        }

        public MissingAttributeAnnotationException(string? message) : base(message)
        {
        }

        public MissingAttributeAnnotationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MissingAttributeAnnotationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
