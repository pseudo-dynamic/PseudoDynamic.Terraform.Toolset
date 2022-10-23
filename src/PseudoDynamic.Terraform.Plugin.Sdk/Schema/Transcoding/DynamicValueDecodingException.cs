using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.Transcoding
{
    [Serializable]
    internal class DynamicValueDecodingException : Exception
    {
        public DynamicValueDecodingException()
        {
        }

        public DynamicValueDecodingException(string? message) : base(message)
        {
        }

        public DynamicValueDecodingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DynamicValueDecodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
