using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class TerraformNameConventionException : Exception
    {
        public static void EnsureProviderNameConvention(string providerName)
        {
            foreach (var namePart in providerName.Split("/")) {
                if (namePart != SnakeCaseConvention.Default.Format(namePart)) {
                    throw new TerraformNameConventionException($"A provider name part \"{namePart}\" must be snake_case");
                }
            }
        }

        public TerraformNameConventionException()
        {
        }

        public TerraformNameConventionException(string? message) : base(message)
        {
        }

        public TerraformNameConventionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TerraformNameConventionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
