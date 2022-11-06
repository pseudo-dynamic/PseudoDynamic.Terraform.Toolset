using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Kestrel
{
    public class ServerCertificateAcceptanceTests
    {
        [Fact.Terraform]
        internal async Task Terraform_validate_works_with_custom_certificate()
        {
            using TerraformCommand.WorkingDirectoryCloning terraform = new(options => options.ForProviderProject("minimal"));
            await terraform.ValidateAsync();
        }
    }
}
