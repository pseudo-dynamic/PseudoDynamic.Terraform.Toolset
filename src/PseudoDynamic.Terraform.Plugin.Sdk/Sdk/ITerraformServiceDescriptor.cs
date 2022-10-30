namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal interface ITerraformServiceDescriptor
    {
        object? Service { get; }
        Type ServiceType { get; }
        Type SchemaType { get; }
    }
}
