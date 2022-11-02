using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Kestrel
{
    public class ServerCertificateAcceptanceTests
    {
        [Fact.Terraform]
        internal async Task Terraform_validate_works_with_custom_certificate()
        {
            using var terraform = new TerraformCommand.WorkingDirectoryCloning(options => options.ForProviderProject("minimal"));
            await terraform.Validate();
        }
    }
}
