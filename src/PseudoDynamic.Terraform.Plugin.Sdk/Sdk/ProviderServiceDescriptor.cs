namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal record class ProviderServiceDescriptor : TerraformServiceDescriptor
    {
        public ProviderServiceDescriptor(Type resourceType, Type schemaType)
           : base(resourceType, schemaType)
        {
        }

        protected override void Validate() => EnsureServiceImplementsGenericTypeDefinition(typeof(IProvider<>), new[] { SchemaType }, "provider");
    }
}
