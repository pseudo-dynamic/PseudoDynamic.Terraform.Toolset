namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class ResourceServiceDescriptor : TerraformServiceDescriptor
    {
        public override Type ProviderMetaSchemaType { get; }

        public ResourceServiceDescriptor(Type resourceType, Type schemaType, Type providerMetaSchemaType)
            : base(resourceType, schemaType)
        {
            ProviderMetaSchemaType = providerMetaSchemaType ?? throw new ArgumentNullException(nameof(providerMetaSchemaType));
            EnsureServiceImplementsGenericTypeDefinition(typeof(IResource<,>), new[] { SchemaType, ProviderMetaSchemaType }, "resource");
        }
    }
}
