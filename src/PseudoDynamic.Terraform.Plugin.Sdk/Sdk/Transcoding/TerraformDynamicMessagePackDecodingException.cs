using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    [Serializable]
    internal class TerraformDynamicMessagePackDecodingException : Exception
    {
        public TerraformDynamicMessagePackDecodingException()
        {
        }

        public TerraformDynamicMessagePackDecodingException(string? message) : base(message)
        {
        }

        public TerraformDynamicMessagePackDecodingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TerraformDynamicMessagePackDecodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
