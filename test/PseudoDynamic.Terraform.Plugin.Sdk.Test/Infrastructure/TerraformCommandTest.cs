using FluentAssertions;
using PseudoDynamic.Terraform.Plugin.Infrastructure.Diagnostics;

namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    public class TerraformCommandTest
    {
        [Fact.Terraform]
        public async Task Asynchronous_terraform_plan_prints_error_message()
        {
            var terraform = new TerraformCommand(static options => { });

            await terraform.Awaiting(command => command.PlanAsync()).Should()
                .ThrowAsync<BadExitCodeException>()
                .WithMessage("Error: No configuration files*Exit Code = 1");
        }

        [Fact.Terraform]
        public void Synchronous_terraform_plan_prints_error_message()
        {
            var terraform = new TerraformCommand(static options => { });

            terraform.Invoking(command => command.Plan()).Should()
                .Throw<BadExitCodeException>()
                .WithMessage("Error: No configuration files*Exit Code = 1");
        }
    }
}
