using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Transcoding
{
    [Serializable]
    internal class TerraformDynamicMessagePackEncodingException : Exception
    {
        public TerraformDynamicMessagePackEncodingException()
        {
        }

        public TerraformDynamicMessagePackEncodingException(string? message) : base(message)
        {
        }

        public TerraformDynamicMessagePackEncodingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TerraformDynamicMessagePackEncodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
