namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ResourceServiceDescriptor : TerraformServiceDescriptor
    {
        public ResourceServiceDescriptor(Type resourceType, Type schemaType)
            : base(resourceType, schemaType)
        {
        }

        protected override void Validate() => EnsureServiceImplementsGenericTypeDefinition(typeof(IResource<>), new[] { SchemaType }, "resource");
    }
}
