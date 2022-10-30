namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal static class TerraformCommandExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="providerProjectName">E.g. "default" or "minimal"</param>
        public static void ForProviderProject(this TerraformCommand.WorkingDirectoryCloning.WorkingDirectoryCloningOptions options, string providerProjectName)
        {
            const string terraformConfigurationFile = "terraform.tfrc";
            options.WorkingDirectory = $"ProviderProjects/{providerProjectName}";

            options.SetCopyableFilePatterns(
                "main.tf",
                terraformConfigurationFile);

            options.TfCliConfigFile = terraformConfigurationFile;
        }
    }
}
