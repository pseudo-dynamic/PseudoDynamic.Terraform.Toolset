namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class DataSourceServiceDescriptor : TerraformServiceDescriptor
    {
        public DataSourceServiceDescriptor(Type resourceType, Type schemaType)
           : base(resourceType, schemaType)
        {
        }

        protected override void Validate() => EnsureServiceImplementsGenericTypeDefinition(typeof(IDataSource<>), new[] { SchemaType }, "data source");
    }
}
