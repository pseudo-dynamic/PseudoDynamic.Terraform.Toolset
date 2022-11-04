using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class TerraformaAcceptanceTests
    {
        [Fact.Terraform]
        internal async Task Terraform_validate_passes()
        {
            using var terraform = new TerraformCommand.WorkingDirectoryCloning(options => options.ForProviderProject("default"));
            await terraform.Validate();
        }

        [Fact.Terraform]
        internal async Task Terraform_plan_passes()
        {
            using var terraform = new TerraformCommand.WorkingDirectoryCloning(options => options.ForProviderProject("default"));
            await terraform.Plan();
        }

        [Fact.Terraform]
        internal async Task Terraform_apply_passes()
        {
            using var terraform = new TerraformCommand.WorkingDirectoryCloning(options => options.ForProviderProject("default"));
            await terraform.Apply();
        }
    }
}
