namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal class AttributePath
    {
        public IList<Step>? Steps { get; set; }

        public class Step
        {
            public SelectorOneOfCase SelectorCase { get; set; }

            public string? AttributeName { get; set; }
            public string? ElementKeyString { get; set; }
            public string? ElementKeyInt { get; set; }

            public enum SelectorOneOfCase
            {
                AttributeName,
                ElementKeyString,
                ElementKeyInt
            }
        }
    }
}
