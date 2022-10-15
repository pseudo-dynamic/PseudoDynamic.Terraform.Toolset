namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ResourceDescriptor
    {
        public string ResourceName { get; }
        public Type ResourceType { get; }
        public Type SchemaType { get; }

        public ResourceDescriptor(string resourceName, Type resourceType, Type schemaType)
        {
            ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            ResourceType = resourceType ?? throw new ArgumentNullException(nameof(resourceType));
            SchemaType = schemaType ?? throw new ArgumentNullException(nameof(schemaType));
        }
    }
}
