using PseudoDynamic.Terraform.Plugin.Infrastructure;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Kestrel
{
    public class TerraformValidateTests
    {
        private string GetConfiguration()
        {
#if DEBUG
            return "Debug";
#else
            return "Release";
#endif
        }

        [Fact]
        internal void Terraform_validate_works_with_custom_certificate()
        {
            var terraformConfigurationFile = "terraform.tfrc";
            var terraformProviderDirectory = $"bin\\{GetConfiguration()}\\net6.0\\publish";
            var terraformProviderName = "terraform-provider-debug";

            using var terraform = new TerraformCommand.WorkingDirectoryCloning(options =>
            {
                options.WorkingDirectory = "CSharpProjects/terraform-validate";

                options.SetCopyableFilePatterns(
                    "main.tf",
                    $"{terraformProviderDirectory}/{terraformProviderName}",
                    $"{terraformProviderDirectory}/{terraformProviderName}.exe");

                options.TfCliConfigFile = terraformConfigurationFile;
            });

            File.WriteAllText(Path.Combine(terraform.WorkingDirectory, terraformConfigurationFile), $$"""
                provider_installation {
                    dev_overrides {
                        "pseudo-dynamic/debug" = "{{Path.Combine(terraform.WorkingDirectory, terraformProviderDirectory).Replace("\\", "/")}}"
                    }
                }
                """);

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            terraform.Validate();
        }
    }
}
