namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class ProviderServiceDescriptor : TerraformServiceDescriptor
    {
        public ProviderServiceDescriptor(Type resourceType, Type schemaType)
           : base(resourceType, schemaType) =>
            EnsureServiceImplementsGenericTypeDefinition(typeof(IProvider<>), new[] { SchemaType }, "provider");
    }
}
