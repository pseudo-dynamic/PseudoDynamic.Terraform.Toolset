namespace PseudoDynamic.Terraform.Plugin.Internals
{
    internal class TerraformReattachProvider
    {
        public string Protocol { get; init; } = "grpc";
        public int Pid { get; init; } = Environment.ProcessId;
        public bool Test { get; init; } = true;
        public TerraformReattachProviderAddress Addr { get; init; }
    }
}