using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    [Serializable]
    internal class TerraformValueException : Exception
    {
        public TerraformValueException()
        {
        }

        public TerraformValueException(string? message) : base(message)
        {
        }

        public TerraformValueException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TerraformValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
