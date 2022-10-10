namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class TerraformReattachProviderAddress
    {
        public string Network { get; init; } = "tcp";
        public string String { get; init; }

        public TerraformReattachProviderAddress(string address) =>
            String = address;
    }
}