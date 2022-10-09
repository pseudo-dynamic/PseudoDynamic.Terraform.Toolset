namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal class SchemaVersionAttribute : Attribute
    {
        public int SchemaVersion { get; }

        public SchemaVersionAttribute(int schemaVersion) =>
            SchemaVersion = schemaVersion;
    }
}
