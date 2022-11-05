using FluentAssertions;
using PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    public class TerraformCommandTest
    {
        [Fact.Terraform]
        public async Task Terraform_plan_prints_error_message()
        {
            var terraform = new TerraformCommand(static options => { });

            await terraform.Awaiting(command => command.Plan()).Should()
                .ThrowAsync<BadExitCodeException>()
                .WithMessage("Error: No configuration files*Exit Code = 1");
        }
    }
}
