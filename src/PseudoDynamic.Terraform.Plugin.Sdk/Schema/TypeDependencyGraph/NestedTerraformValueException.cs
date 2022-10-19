using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    [Serializable]
    internal class NestedTerraformValueException : TerraformValueException
    {
        public NestedTerraformValueException()
        {
        }

        public NestedTerraformValueException(string? message) : base(message)
        {
        }

        public NestedTerraformValueException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NestedTerraformValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
