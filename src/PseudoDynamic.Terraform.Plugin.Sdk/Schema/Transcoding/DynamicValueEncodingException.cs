using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.Transcoding
{
    [Serializable]
    internal class DynamicValueEncodingException : Exception
    {
        public DynamicValueEncodingException()
        {
        }

        public DynamicValueEncodingException(string? message) : base(message)
        {
        }

        public DynamicValueEncodingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DynamicValueEncodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
