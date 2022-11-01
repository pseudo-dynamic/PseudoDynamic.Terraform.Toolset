namespace PseudoDynamic.Terraform.Plugin.Sdk.Services
{
    internal interface ITerraformServiceDescriptor
    {
        object? Implementation { get; }
        Type ImplementationType { get; }
        Type SchemaType { get; }
    }
}
