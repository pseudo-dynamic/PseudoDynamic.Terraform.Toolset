using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class TerraformaAcceptanceTests
    {
        private readonly ITestOutputHelper _output;

        public TerraformaAcceptanceTests(ITestOutputHelper output) =>
            _output = output ?? throw new ArgumentNullException(nameof(output));

        [Fact]
        internal async Task Terraform_validate_passes()
        {
            using var terraform = new TerraformCommand.WorkingDirectoryCloning(options => options.ForProviderProject("default"));
            await terraform.Validate();
        }

        [Fact]
        internal async Task Terraform_plan_passes()
        {
            using var terraform = new TerraformCommand.WorkingDirectoryCloning(options => options.ForProviderProject("default"));
            await terraform.Plan();
        }
    }
}
