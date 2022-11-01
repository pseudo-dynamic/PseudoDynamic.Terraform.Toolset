namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal record class DataSourceServiceDescriptor : TerraformServiceDescriptor
    {
        public override Type ProviderMetaSchemaType { get; }

        public DataSourceServiceDescriptor(Type resourceType, Type schemaType, Type providerMetaSchemaType)
           : base(resourceType, schemaType)
        {
            ProviderMetaSchemaType = providerMetaSchemaType ?? throw new ArgumentNullException(nameof(providerMetaSchemaType));
            EnsureServiceImplementsGenericTypeDefinition(typeof(IDataSource<,>), new[] { SchemaType, ProviderMetaSchemaType }, "data source");
        }
    }
}
