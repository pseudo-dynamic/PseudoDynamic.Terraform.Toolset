namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal static class StopProvider
    {
        public class Request
        {
        }

        public class Response
        {
            public string? Error { get; set; }
        }
    }
}
