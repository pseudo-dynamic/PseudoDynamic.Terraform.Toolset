namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
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
